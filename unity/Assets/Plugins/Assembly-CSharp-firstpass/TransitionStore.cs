using System.Collections.Generic;
using UnityEngine;

public class TransitionStore : MonoBehaviour, IAnimatable
{
	protected EZTransitionList m_transitions;

	[HideInInspector]
	public EZTransitionList[] m_transitionsArray = new EZTransitionList[1];

	[HideInInspector]
	public string[] m_states = new string[1] { "Animation" };

	public EZTransitionList[] Transitions
	{
		get
		{
			return m_transitionsArray;
		}
		set
		{
			m_transitionsArray = value;
		}
	}

	protected virtual void Awake()
	{
		m_transitions = m_transitionsArray[0];
	}

	protected virtual void Start()
	{
		m_transitions = m_transitionsArray[0];
	}

	public EZTransition FindTransition(string _name)
	{
		// A list item can query its transitions while its children have not
		// yet received Awake (the scroll list toggles cloned hierarchies).
		if (m_transitions == null) m_transitions = m_transitionsArray[0];
		for (int i = 0; i < m_transitions.list.Length; i++)
		{
			if (m_transitions.list[i].name == _name)
			{
				return m_transitions.list[i];
			}
		}
		return null;
	}

	public EZTransition GetTransition(int _index)
	{
		if (m_transitions == null) m_transitions = m_transitionsArray[0];
		return m_transitions.list[_index];
	}

	public int DrawPreStateSelectGUI(int selState, bool inspector)
	{
		return 0;
	}

	public int DrawPostStateSelectGUI(int selState)
	{
		return 0;
	}

	public void DrawPreTransitionUI(int selState, IGUIScriptSelector gui)
	{
	}

	public void DrawPreTransitionUI(int selState, int transitionIndex)
	{
		EZTransitionList eZTransitionList = m_transitionsArray[selState];
		GUILayout.BeginHorizontal(GUILayout.MaxWidth(200f));
		GUILayout.Label("Animation Name:");
		GUILayout.Space(5f);
		if (transitionIndex < eZTransitionList.list.Length)
		{
			eZTransitionList.list[transitionIndex].name = GUILayout.TextField(eZTransitionList.list[transitionIndex].name);
		}
		else
		{
			GUILayout.TextField("No Anims");
		}
		GUILayout.Space(20f);
		if (GUILayout.Button("Add Animation", "Button"))
		{
			int num = eZTransitionList.list.Length;
			string text = "Temp";
			bool flag = false;
			while (!flag)
			{
				flag = true;
				text = "Anim " + num;
				EZTransition[] list = eZTransitionList.list;
				foreach (EZTransition eZTransition in list)
				{
					if (eZTransition.name == text)
					{
						flag = false;
						break;
					}
				}
				num++;
			}
			List<EZTransition> list2 = new List<EZTransition>();
			list2.AddRange(eZTransitionList.list);
			EZTransition eZTransition2 = new EZTransition(text);
			eZTransition2.initialized = true;
			list2.Insert(eZTransitionList.list.Length, eZTransition2);
			eZTransitionList.list = list2.ToArray();
		}
		if (GUILayout.Button("Delete Animation", "Button") && transitionIndex < eZTransitionList.list.Length)
		{
			List<EZTransition> list3 = new List<EZTransition>();
			list3.AddRange(eZTransitionList.list);
			list3.RemoveAt(transitionIndex);
			eZTransitionList.list = list3.ToArray();
		}
		GUILayout.EndHorizontal();
	}

	public string[] EnumStateElements()
	{
		return m_states;
	}

	public EZTransitionList GetTransitions(int index)
	{
		if (index >= m_transitionsArray.Length)
		{
			return null;
		}
		return m_transitionsArray[index];
	}

	public string GetStateLabel(int index)
	{
		return m_states[index];
	}

	public void SetStateLabel(int index, string label)
	{
		m_states[index] = label;
	}

	public ASCSEInfo GetStateElementInfo(int stateNum)
	{
		ASCSEInfo aSCSEInfo = new ASCSEInfo();
		aSCSEInfo.transitions = GetTransitions(stateNum);
		return aSCSEInfo;
	}
}
