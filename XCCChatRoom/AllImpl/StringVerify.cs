namespace XCCChatRoom.AllImpl
{
    public static class StringVerify
    {
        public static string[] ContrabandVocabulary = new string[] {  };
        public static bool VerifyString(this string verifyString)
        {
            int i = 1;
            foreach (string word in ContrabandVocabulary)
            {
                if (verifyString == word)
                {
                    i = -1;
                    break;
                }
            }
            if (i > 0)
                return true;
            else
                return false;
        }

        public static bool UserNameEditor(this string NewUserName)
        {
                if (!NewUserName.Contains(' ') && NewUserName.VerifyString()) { return true; }
                else { return false; }
        }

        public static bool PasswordEditor(this string NewPassword)
        {
            if (NewPassword is not null && NewPassword != string.Empty)
            {
                if (!NewPassword.Contains(' ') && NewPassword.Length < 20) { return true; }
                else { return false; }
            }
            else { return false; }
        }
    }
}
