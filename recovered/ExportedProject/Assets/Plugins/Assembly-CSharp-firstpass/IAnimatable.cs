public interface IAnimatable
{
	EZTransitionList[] Transitions { get; set; }

	int DrawPreStateSelectGUI(int selState, bool inspector);

	int DrawPostStateSelectGUI(int selState);

	void DrawPreTransitionUI(int selState, IGUIScriptSelector gui);

	void DrawPreTransitionUI(int selState, int transitionIndex);

	string[] EnumStateElements();

	EZTransitionList GetTransitions(int index);

	string GetStateLabel(int index);

	void SetStateLabel(int index, string label);

	ASCSEInfo GetStateElementInfo(int stateNum);
}
