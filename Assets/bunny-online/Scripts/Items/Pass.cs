
using UnityEngine;

namespace Items
{
    [CreateAssetMenu (fileName = "ScriptablePass", menuName = "ScriptableObjects/Pass")]
    public class Pass : Item
    {
         public bool isCorrect;

         //for debug
         public override string GetProperties()
         {
             return base.GetProperties() + "valid: "+isCorrect;
         }
    }
}
