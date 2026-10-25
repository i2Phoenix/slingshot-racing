using System.Runtime.InteropServices;
using System.Text;

public class OpenGL
{
	public const uint GL_POINTS = 0u;

	public const uint GL_LINES = 1u;

	public const uint GL_LINE_LOOP = 2u;

	public const uint GL_LINE_STRIP = 3u;

	public const uint GL_TRIANGLES = 4u;

	public const uint GL_TRIANGLE_STRIP = 5u;

	public const uint GL_TRIANGLE_FAN = 6u;

	public const uint GL_BYTE = 5120u;

	public const uint GL_UNSIGNED_BYTE = 5121u;

	public const uint GL_SHORT = 5122u;

	public const uint GL_UNSIGNED_SHORT = 5123u;

	public const uint GL_FLOAT = 5126u;

	public const uint GL_FIXED = 5132u;

	public const uint GL_ARRAY_BUFFER = 34962u;

	public const uint GL_ELEMENT_ARRAY_BUFFER = 34963u;

	public const uint GL_ARRAY_BUFFER_BINDING = 34964u;

	public const uint GL_ELEMENT_ARRAY_BUFFER_BINDING = 34965u;

	public const uint GL_FRAGMENT_SHADER = 35632u;

	public const uint GL_VERTEX_SHADER = 35633u;

	public const uint GL_MAX_VERTEX_ATTRIBS = 34921u;

	public const uint GL_MAX_VERTEX_UNIFORM_VECTORS = 36347u;

	public const uint GL_MAX_VARYING_VECTORS = 36348u;

	public const uint GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS = 35661u;

	public const uint GL_MAX_VERTEX_TEXTURE_IMAGE_UNITS = 35660u;

	public const uint GL_MAX_TEXTURE_IMAGE_UNITS = 34930u;

	public const uint GL_MAX_FRAGMENT_UNIFORM_VECTORS = 36349u;

	public const uint GL_SHADER_TYPE = 35663u;

	public const uint GL_DELETE_STATUS = 35712u;

	public const uint GL_LINK_STATUS = 35714u;

	public const uint GL_VALIDATE_STATUS = 35715u;

	public const uint GL_ATTACHED_SHADERS = 35717u;

	public const uint GL_ACTIVE_UNIFORMS = 35718u;

	public const uint GL_ACTIVE_UNIFORM_MAX_LENGTH = 35719u;

	public const uint GL_ACTIVE_ATTRIBUTES = 35721u;

	public const uint GL_ACTIVE_ATTRIBUTE_MAX_LENGTH = 35722u;

	public const uint GL_SHADING_LANGUAGE_VERSION = 35724u;

	public const uint GL_CURRENT_PROGRAM = 35725u;

	public const uint GL_VERTEX_ATTRIB_ARRAY_ENABLED = 34338u;

	public const uint GL_VERTEX_ATTRIB_ARRAY_SIZE = 34339u;

	public const uint GL_VERTEX_ATTRIB_ARRAY_STRIDE = 34340u;

	public const uint GL_VERTEX_ATTRIB_ARRAY_TYPE = 34341u;

	public const uint GL_VERTEX_ATTRIB_ARRAY_NORMALIZED = 34922u;

	public const uint GL_VERTEX_ATTRIB_ARRAY_POINTER = 34373u;

	public const uint GL_VERTEX_ATTRIB_ARRAY_BUFFER_BINDING = 34975u;

	private const string Library = "libGLESv2";

	[DllImport("libGLESv2")]
	public static extern void glDrawArrays(uint mode, int first, int count);

	[DllImport("libGLESv2")]
	public static extern void glGetIntegerv(uint pname, out int _params);

	[DllImport("libGLESv2")]
	public static extern void glGetVertexAttribiv(uint index, uint pname, out int _params);

	[DllImport("libGLESv2")]
	public static extern void glEnableVertexAttribArray(uint index);

	[DllImport("libGLESv2")]
	public static extern void glDisableVertexAttribArray(uint index);

	[DllImport("libGLESv2")]
	public static extern void glVertexAttrib4f(uint index, float v0, float v1, float v2, float v3);

	[DllImport("libGLESv2")]
	public static extern void glBindBuffer(uint target, uint buffer);

	[DllImport("libGLESv2")]
	public static extern void glGetProgramiv(uint program, uint pname, out int _params);

	[DllImport("libGLESv2")]
	public static extern void glGetActiveUniform(uint program, uint index, int bufSize, out int length, out int size, out uint type, [MarshalAs(UnmanagedType.LPStr)] StringBuilder name);

	[DllImport("libGLESv2")]
	public static extern void glGetActiveAttrib(uint program, uint index, int bufSize, out int length, out int size, out uint type, [MarshalAs(UnmanagedType.LPStr)] StringBuilder name);

	[DllImport("libGLESv2")]
	public static extern int glGetAttribLocation(uint program, [MarshalAs(UnmanagedType.LPStr)] string name);
}
