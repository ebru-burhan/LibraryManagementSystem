namespace Library.Entity.Constants;

public static class ActionTypes
{
    public static class Crud
    {
        public const string Create = "CREATE";
        public const string Update = "UPDATE";
        public const string Delete = "DELETE";
    }

    public static class System
    {
        public const string Login = "LOGIN";
        public const string Logout = "LOGOUT";
    }

    public static class Library
    {
        public const string Borrow = "BORROW";
        public const string Return = "RETURN";
        public const string Approve = "APPROVE";
        public const string Reject = "REJECT";
    }
}