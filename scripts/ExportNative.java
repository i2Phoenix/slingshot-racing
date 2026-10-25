import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.DecompInterface;
import ghidra.app.decompiler.DecompileResults;
import ghidra.program.model.listing.Function;
import ghidra.program.model.listing.Parameter;
import ghidra.program.model.listing.ParameterImpl;
import ghidra.program.model.data.*;
import ghidra.program.model.symbol.SourceType;
import java.io.PrintWriter;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.*;

/** Export the game's named native functions to an evidence file. */
public class ExportNative extends GhidraScript {
    private final Map<String, DataType> types = new HashMap<>();

    private DataType type(String name) {
        if (name.endsWith("*")) return new PointerDataType(type(name.substring(0, name.length()-1)), 4);
        DataType value = types.get(name);
        if (value == null) throw new IllegalArgumentException(name);
        return value;
    }

    public void run() throws Exception {
        if (getScriptArgs().length >= 3) {
            types.put("void", VoidDataType.dataType);
            types.put("float", FloatDataType.dataType);
            types.put("int", IntegerDataType.dataType);
            types.put("uint", UnsignedIntegerDataType.dataType);
            types.put("ushort", UnsignedShortDataType.dataType);
            types.put("byte", ByteDataType.dataType);
            types.put("bool", BooleanDataType.dataType);
            for (String line : Files.readAllLines(Path.of(getScriptArgs()[2]))) {
                String[] fields = line.split("\t");
                StructureDataType structure = new StructureDataType(fields[0], 0);
                for (int i=1; i<fields.length; i++) {
                    String[] field = fields[i].split(":");
                    structure.add(type(field[0]), field[1], null);
                }
                types.put(fields[0], currentProgram.getDataTypeManager().addDataType(structure, DataTypeConflictHandler.REPLACE_HANDLER));
            }
            List<Function> functions = new ArrayList<>();
            currentProgram.getFunctionManager().getFunctions(true).forEachRemaining(functions::add);
            currentProgram.getFunctionManager().getExternalFunctions().forEachRemaining(functions::add);
            for (String line : Files.readAllLines(Path.of(getScriptArgs()[1]))) {
                String[] fields = line.split("\t");
                for (Function function : functions) {
                    if (!function.getName().equals(fields[1])) continue;
                    // Android armeabi-v7a uses the soft-float ABI even when the
                    // implementation uses VFP instructions internally.
                    function.setCallingConvention("__stdcall_softfp");
                    List<Parameter> parameters = new ArrayList<>();
                    for (int i=2; i<fields.length; i++) {
                        String[] field = fields[i].split(":");
                        parameters.add(new ParameterImpl(field[1], type(field[0]), currentProgram));
                    }
                    function.setReturnType(type(fields[0]), SourceType.USER_DEFINED);
                    function.replaceParameters(parameters, Function.FunctionUpdateType.DYNAMIC_STORAGE_ALL_PARAMS, true, SourceType.USER_DEFINED);
                }
            }
        }
        DecompInterface decompiler = new DecompInterface();
        decompiler.openProgram(currentProgram);
        try (PrintWriter out = new PrintWriter(getScriptArgs()[0])) {
            for (Function function : currentProgram.getFunctionManager().getFunctions(true)) {
                String name = function.getName();
                if (!(name.startsWith("TrackUtils_") || name.startsWith("CubicHermite") ||
                      name.startsWith("RopeUpdate") || name.startsWith("ParticleSys_") ||
                      name.startsWith("Streamers_") || name.equals("QuaternionMul"))) continue;
                if (name.endsWith("_Simd")) continue;
                DecompileResults result = decompiler.decompileFunction(function, 60, monitor);
                out.println("\n// " + function.getEntryPoint() + " " + name);
                if (result.decompileCompleted()) out.println(result.getDecompiledFunction().getC());
                else out.println("// FAILED: " + result.getErrorMessage());
            }
        } finally {
            decompiler.dispose();
        }
    }
}
