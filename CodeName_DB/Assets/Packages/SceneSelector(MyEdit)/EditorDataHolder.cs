using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//namespace ColorGuardian.EditorTools
//{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EditorDataHolder", order = 1)]
    public class EditorDataHolder : ScriptableObject
    {
        public string testScene;
        public bool usetestScene;
    }
//}
