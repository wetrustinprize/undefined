using UnityEngine;
using UnityEditor;
using Untitled.Motor;
using System.Collections.Generic;

public class MotorForcesWindow : EditorWindow
{

        #region Variables

    // Layout stuff
    float headerHeight = 50f;
    float tableMargin = 1f;
    Rect headerRect;
    Rect[] tableRect;

    // Motor stuff
    GameObject motorOwner;
    Motor motorToInspect;
    List<KeyValuePair<bool, Force>> forces = new List<KeyValuePair<bool, Force>>();

        #endregion

    [MenuItem("Window/Motor/Forces")]
    static void OpenWindow() {
        MotorForcesWindow window = (MotorForcesWindow)GetWindow(typeof(MotorForcesWindow));
        window.minSize = new Vector2(300, 400);
        window.Show();
    }

    void OnEnable() {
        TryGetMotor();
    }

    void OnGUI() {
        MakeLayouts();
        DrawHeader();
        DrawMain();
    }

    void Update() {
        GetForces();
    }

    void MakeLayouts() {

        headerRect.x = 0;
        headerRect.y = 0;
        headerRect.width = Screen.width;
        headerRect.height = headerHeight;

        int t = 6;
        tableRect = new Rect[t];
        for(int i = 1; i <= t; i++)
        {

            Rect newRect = new Rect(

                (Screen.width / t) * (i - 1) + tableMargin,
                headerHeight,
                Screen.width - tableMargin,
                Screen.height - headerHeight

            );

            tableRect[i-1] = newRect;

        }

    }

    void DrawHeader() {

        GUIStyle title = new GUIStyle();
        title.fontSize = 16;
        title.alignment = TextAnchor.MiddleCenter;

        GUIStyle info = new GUIStyle();
        info.alignment = TextAnchor.MiddleCenter;

        GUILayout.BeginArea(headerRect);

            GUILayout.Label("Motor Forces Information", title);
            GUILayout.Label("Owner: " + motorOwner.name, info);
            if(motorToInspect == null)
                GUILayout.Label("No motor found", info);
        
        GUILayout.EndArea();

    }

    void OnSelectionChange() {

        TryGetMotor();
        Repaint();

    }

    void DrawMain() {

        if(motorToInspect != null)
            DrawForceTable();

    }

    void DrawForceTable() {
        GUIStyle columnTextStyle = new GUIStyle();
        columnTextStyle.fontSize = 14;
        //columnTextStyle.alignment = TextAnchor.UpperCenter;

        string[] columns = new string[] {
            "Type",
            "Name",
            "Start Force",
            "Actual Force",
            "Timer",
            "Gravity"
        };

        for(int i = 0; i < tableRect.Length; i++)
        {

            GUILayout.BeginArea(tableRect[i]);

                GUILayout.BeginVertical();

                    GUILayout.Label(columns[i], columnTextStyle);

                    if(forces != null) {
                        foreach(var f in forces)
                        {
                            bool isExternal = f.Key;
                            Force force = f.Value;

                            string l = "";

                            switch(i) {
                                case 0:
                                    l = isExternal ? "External" : "Constant";
                                    break;
                                case 1:
                                    l = force.Name;
                                    break;
                                case 2:
                                    l = force.ForceApplied.ToString();
                                    break;
                                case 3:
                                    l = force.ActualForce.ToString();
                                    break;
                                case 4:
                                    l = force.TimeToStop.ToString();
                                    break;
                                case 5:
                                    l = force.ApplyGravity.ToString();
                                    break;
                            }

                            GUILayout.Label(l);

                        }
                    }

                GUILayout.EndVertical();
            GUILayout.EndArea();

        }

    }

    void TryGetMotor() {

        try {

            motorOwner = (GameObject)Selection.activeObject;
            motorToInspect = motorOwner.GetComponent<Motor>();
            forces = new List<KeyValuePair<bool, Force>>();

        } catch { }

    }

    void GetForces() {
        if(motorToInspect == null) return;
        //forces = new List<KeyValuePair<bool, Force>>();
        List<Force> constForce = motorToInspect.constantForces;
        List<Force> extForce = motorToInspect.externalForces;

        if(constForce.Count > 0) {
            foreach(Force f in constForce) {
                KeyValuePair<bool, Force> value = new KeyValuePair<bool, Force>(false, f);
                forces.Add(value);
            }
        }

        if(extForce.Count > 0) {
            foreach(Force f in extForce) {
                KeyValuePair<bool, Force> value = new KeyValuePair<bool, Force>(true, f);
                forces.Add(value);
            }
        }
        Repaint();

    }

}
