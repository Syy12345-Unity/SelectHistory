using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Syy.SelectHistory {

    [InitializeOnLoad]
    public class SelectHistory
    {
        const int MAX_HISTORY_COUNT = 30;
        static List<HistoryData> history = new List<HistoryData>();
        static int select = 0;
        static bool edit = false;
        static SelectHistory() {
            history.Clear();
            select = 0;
            Selection.selectionChanged += SelectionChanged;
        }
        static void SelectionChanged()
        {
            if(edit) {
                edit = false;
                return;
            }
                
            history.Add(new HistoryData(Selection.objects));
            history.RemoveAll(data => data.IsKilled);
            if(history.Count > MAX_HISTORY_COUNT) {
                history.RemoveAt(0);
            }
            select = history.Count;
        }

        [MenuItem("Edit/History/Next %#l")]
        static void Next() {
            int a = select + 1;
            HistoryData data = null;
            while (a >= 1 && a <= history.Count)
            {
                data = history[a - 1];
                if (data.IsKilled)
                {
                    data = null;
                    a++;
                }
                else
                {
                    break;
                }
            }

            if (data != null)
            {
                edit = true;
                Selection.objects = data.Objects;
                if(data.Objects.Length >= 1) {
                    EditorGUIUtility.PingObject(data.Objects[0]);
                }
                select = a;
            }
        }

        [MenuItem("Edit/History/Back %#h")]
        static void Back()
        {
            int a = select -1;
            HistoryData data = null;
            while(a >= 1 && a <= history.Count) {
                data = history[a - 1];
                if(data.IsKilled) {
                    data = null;
                    a--;
                } else {
                    break;
                }
            }

            if(data != null) {
                edit = true;
                Selection.objects = data.Objects;
                if (data.Objects.Length >= 1)
                {
                    EditorGUIUtility.PingObject(data.Objects[0]);
                }
                select = a;
            }
        }

        public class HistoryData {
            public Object[] Objects { get; private set; }
            public HistoryData(Object[] objects) {
                Objects = objects;
            }

            public HistoryData(Object obj) {
                Objects = new Object[] { obj };
            }

            public bool IsKilled {
                get {
                    return Objects.All(data => data == null);
                }
            }
        }
    }

}
