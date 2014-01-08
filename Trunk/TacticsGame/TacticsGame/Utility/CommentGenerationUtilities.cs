using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Utility
{
    public static class CommentGenerationUtilities
    {
        private static string[] genericVisitorComments = new string[] 
        {
            "Hey there buddy! How are things going?",
            "I ran into a team of those rangers in the woods. Amazing woodsmen!",
            "Take a word of advice from me... don't rely on those lazy workers!",        
        };

        public static string GenerateGenericVisitorComment() 
        {
            return genericVisitorComments.GetRandomItem();
        }
    }
}
