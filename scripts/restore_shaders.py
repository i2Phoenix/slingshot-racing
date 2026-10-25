#!/usr/bin/env python3
"""Translate the APK's GLES shaders to editable Unity HLSL, preserving render states.

Run with .venv/bin/python. Requires glslangValidator and spirv-cross.
Original shader text and intermediate GLSL/SPIR-V remain available for review.
"""
import collections
import hashlib
import json
from pathlib import Path
import re
import subprocess

import UnityPy

ROOT = Path(__file__).resolve().parents[1]
WORK = ROOT / "work/shaders"
ORIGINAL = ROOT / "recovered/OriginalShaders"
BUILTINS = {
    "glstate_matrix_mvp": "UNITY_MATRIX_MVP",
    "glstate_matrix_modelview0": "UNITY_MATRIX_MV",
    "glstate_matrix_projection": "UNITY_MATRIX_P",
    "_Object2World": "unity_ObjectToWorld",
    "_World2Object": "unity_WorldToObject",
    "unity_Scale": "float4(1, 1, 1, 1)",
}
UNITY_UNIFORMS = {"_WorldSpaceCameraPos", "_WorldSpaceLightPos0", "_ScreenParams", "_ProjectionParams", "_Time", "_SinTime", "_CosTime", "unity_DeltaTime", "unity_LightmapST", "unity_Lightmap", "unity_LightmapInd", "unity_SHAr", "unity_SHAg", "unity_SHAb", "unity_SHBr", "unity_SHBg", "unity_SHBb", "unity_SHC"}


def run(args):
    result = subprocess.run(args, capture_output=True, text=True)
    if result.returncode:
        raise RuntimeError(" ".join(map(str, args)) + "\n" + result.stdout + result.stderr)
    return result.stdout


def block_end(text, opening):
    depth, quoted, escaped = 0, False, False
    for i in range(opening, len(text)):
        c = text[i]
        if quoted:
            if escaped:
                escaped = False
            elif c == "\\":
                escaped = True
            elif c == '"':
                quoted = False
        elif c == '"':
            quoted = True
        elif c == "{":
            depth += 1
        elif c == "}":
            depth -= 1
            if depth == 0:
                return i + 1
    raise ValueError("Unclosed ShaderLab block")


def translate_stage(source, stage, directory):
    macro = "VERTEX" if stage == "vert" else "FRAGMENT"
    code = "#version 450\n#define " + macro + "\n" + source
    code = re.sub(r"\b(lowp|mediump|highp)\b", "", code)
    code = re.sub(r"\battribute\b", "in", code)
    code = re.sub(r"\bvarying\b", "out" if stage == "vert" else "in", code)
    code = code.replace("texture2D", "texture").replace("textureCube", "texture")
    if stage == "frag":
        code = code.replace("#version 450", "#version 450\nlayout(location=0) out vec4 fragmentColor;")
        code = code.replace("gl_FragData[0]", "fragmentColor").replace("gl_FragColor", "fragmentColor")
    path = directory / ("source." + stage)
    path.write_text(code)
    run(["glslangValidator", "-G", "--auto-map-bindings", "--auto-map-locations", str(path), "-o", str(path) + ".spv"])
    hlsl = run(["spirv-cross", str(path) + ".spv", "--hlsl", "--shader-model", "30"])
    (directory / (stage + ".raw.hlsl")).write_text(hlsl)
    # Shader model 3 output includes a Direct3D 9 pixel-centre correction. Unity
    # handles platform rasterization itself; do not apply this correction twice.
    hlsl = "\n".join(line for line in hlsl.splitlines() if "gl_HalfPixel" not in line)
    matrix_names = re.findall(r"^uniform float\dx\d (\w+);", hlsl, re.M)
    lines = []
    for line in hlsl.splitlines():
        uniform = re.match(r"uniform \w+ (\w+)(?:\[.*\])?;", line)
        if uniform and (uniform[1] in BUILTINS or uniform[1] in UNITY_UNIFORMS):
            continue
        if not uniform:
            # SPIRV-Cross expresses GLSL column-vector matrices as row-vector
            # HLSL operations. Unity uploads matrices for its usual mul(M, v).
            for name in matrix_names:
                line = re.sub(r"\b" + name + r"\b", "transpose(" + BUILTINS.get(name, name) + ")", line)
            for name, replacement in BUILTINS.items():
                if name not in matrix_names:
                    line = re.sub(r"\b" + name + r"\b", replacement, line)
        lines.append(line)
    hlsl = "\n".join(lines)
    # Modern Unity supplies SH coefficients in linear space, even for Gamma
    # projects. Its helper performs the final conversion expected by these
    # legacy materials; the compiled Unity 3 polynomial did not need it.
    hlsl = hlsl.replace("(x1 + x2) + x3", "ShadeSH9(normal_i0)")
    prefix = "SRVertex" if stage == "vert" else "SRFragment"
    hlsl = hlsl.replace("SPIRV_Cross_Input", prefix + "Input").replace("SPIRV_Cross_Output", prefix + "Output")
    hlsl = re.sub(r"\bmain\(", prefix + "(", hlsl)
    if stage == "vert":
        semantics = {"_glesVertex": "POSITION", "_glesNormal": "NORMAL", "_glesTangent": "TANGENT", "_glesColor": "COLOR"}
        semantics.update({"_glesMultiTexCoord" + str(i): "TEXCOORD" + str(i) for i in range(8)})
        for name, semantic in semantics.items():
            hlsl = re.sub(r"(\b" + name + r"\s*:\s*)\w+", r"\g<1>" + semantic, hlsl)
        hlsl = re.sub(r"(gl_Position\s*:\s*)POSITION", r"\g<1>SV_POSITION", hlsl)
    return hlsl


def translate_program(body, shader_name, index):
    variants = []
    for match in re.finditer(r'SubProgram\s+"gles\s*"\s*\{', body):
        sub = body[match.start():block_end(body, body.index("{", match.start()))]
        source = re.search(r'"!!GLES\n(.*?)"', sub, re.S)
        if not source:
            continue
        keywords_match = re.search(r"Keywords\s*\{(.*?)\}", sub, re.S)
        keywords = re.findall(r'"([^"]+)"', keywords_match[1]) if keywords_match else []
        if not any(v[0] == keywords for v in variants):
            variants.append((keywords, source[1]))
    if not variants:
        raise ValueError("No GLES vertex/fragment source for " + shader_name)
    chunks = ["CGPROGRAM", "#pragma vertex SRVertex", "#pragma fragment SRFragment", "#pragma target 3.0", '#include "UnityCG.cginc"']
    keywords = sorted({key for keys, _ in variants for key in keys})
    if keywords:
        # ShaderLab subprograms describe combinations of independent keyword
        # axes. A single multi_compile line makes them mutually exclusive.
        groups = [
            ["DISABLE_CONSTANT_EXTRACTION", "ENABLE_CONSTANT_EXTRACTION"],
            ["DIRECTIONAL", "POINT", "SPOT", "POINT_COOKIE", "DIRECTIONAL_COOKIE"],
            ["LIGHTMAP_OFF", "LIGHTMAP_ON"], ["DIRLIGHTMAP_OFF", "DIRLIGHTMAP_ON"],
            ["SHADOWS_OFF", "SHADOWS_SCREEN"], ["__", "VERTEXLIGHT_ON"],
        ]
        emitted = set()
        for group in groups:
            active = [key for key in group if key == "__" or key in keywords]
            if any(key != "__" for key in active):
                chunks.append("#pragma multi_compile " + " ".join(active))
                emitted.update(active)
        for key in set(keywords) - emitted:
            chunks.append("#pragma multi_compile __ " + key)
    for variant_index, (keys, code) in enumerate(variants):
        directory = WORK / re.sub(r"[^\w.-]", "_", shader_name) / f"{index}-{variant_index}"
        directory.mkdir(parents=True, exist_ok=True)
        if len(variants) > 1:
            chunks.append(("#if " if variant_index == 0 else "#elif ") + (" && ".join("defined(" + key + ")" for key in keys) or "1"))
        vertex = translate_stage(code, "vert", directory)
        fragment = translate_stage(code, "frag", directory)
        # SPIRV-Cross numbers each stage independently. An unused vertex output
        # (BakedBlobShadow2) shifts UV slots; bind fragment inputs by GLSL name.
        vertex_output = re.search(r"struct SRVertexOutput\s*\{(.*?)\};", vertex, re.S)[1]
        semantics = dict(re.findall(r"\b\w+\s+(\w+)\s*:\s*(\w+)\s*;", vertex_output))
        def match_inputs(match):
            return re.sub(r"(\b\w+\s+)(\w+)(\s*:\s*)(\w+)(\s*;)",
                          lambda field: field[1] + field[2] + field[3] + semantics.get(field[2], field[4]) + field[5],
                          match[0])
        fragment = re.sub(r"struct SRFragmentInput\s*\{.*?\};", match_inputs, fragment, flags=re.S)
        # Globals shared by both entry points must have one declaration.
        declarations = set(re.findall(r"^(?:uniform|static) [^;]+;", vertex, re.M))
        fragment = "\n".join(line for line in fragment.splitlines() if line not in declarations)
        chunks.extend([vertex, fragment])
    if len(variants) > 1:
        chunks.append("#endif")
    chunks.append("ENDCG")
    return "\n".join(chunks)


def main():
    WORK.mkdir(parents=True, exist_ok=True)
    ORIGINAL.mkdir(parents=True, exist_ok=True)
    env = UnityPy.load(str(ROOT / "work/apk/assets/bin/Data"))
    originals, inventory = {}, []
    for obj in env.objects:
        if obj.type.name != "Shader":
            continue
        data = obj.read()
        text = data.m_Script
        if isinstance(text, bytes):
            text = text.decode("utf-8")
        name = re.search(r'Shader\s+"([^"]+)"', text)[1]
        digest = hashlib.sha256(text.encode()).hexdigest()
        path = ORIGINAL / (re.sub(r"[^\w.-]", "_", name) + "-" + digest[:12] + ".shader.txt")
        path.write_text(text)
        if name in originals and originals[name] != text:
            raise ValueError("Different originals share shader name " + name)
        originals[name] = text
        inventory.append({"name": name, "file": str(path.relative_to(ROOT)), "sha256": digest})
    report = {"originals": inventory, "translated": [], "fixed_function": [], "errors": []}
    for path in sorted((ROOT / "unity/Assets/Shader").glob("*.shader")):
        name = re.search(r'Shader\s+"([^"]+)"', path.read_text())[1]
        try:
            text = originals[name]
            replacements = []
            for index, match in enumerate(re.finditer(r'Program\s+"(vp|fp)"\s*\{', text)):
                end = block_end(text, text.index("{", match.start()))
                replacement = translate_program(text[match.start():end], name, index) if match[1] == "vp" else ""
                replacements.append((match.start(), end, replacement))
            for start, end, replacement in reversed(replacements):
                text = text[:start] + replacement + text[end:]
            text = re.sub(r"\s*TexGen\s+ObjectLinear", "", text)
            path.write_text(text + "\n")
            report["translated" if replacements else "fixed_function"].append(name)
        except Exception as error:
            report["errors"].append({"name": name, "error": str(error)})
    (ROOT / "work/shader-restoration.json").write_text(json.dumps(report, indent=2))
    print(json.dumps({key: len(value) for key, value in report.items()}, indent=2))
    for error in report["errors"]:
        print(error["name"], error["error"][-1600:])
    if report["errors"]:
        raise SystemExit(1)


if __name__ == "__main__":
    main()
