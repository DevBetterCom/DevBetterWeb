namespace DevBetterWeb.Web
{
    public static class Constants
    {
        public static class Roles
        {
            public const string ADMINISTRATORS = "Administrators";
            public const string MEMBERS = "Members";

            public const string ADMINISTRATORS_MEMBERS = "Administrators,Members";
        }

        public const int MAX_UPLOAD_FILE_SIZE = 1_512_428_800; // 1500 MB
        public const string FILE_DATE_FORMAT_STRING = "yyyy-MM-dd";
    }
}
