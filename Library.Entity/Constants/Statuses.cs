namespace Library.Entity.Constants;

public static class Statuses
{
    //Statuses.Loan.Overdue şeklinde çağırıcaz nested class sayesinde kalabalık olmadı solution Expl da
    //switch case derlemede çalışır ve const ile derlemede en azından yazım hatasından hata almayı engelledik.
    public static class Loan
    {
        public const string Borrowed = "BORROWED";
        public const string Approaching = "APPROACHING";
        public const string Overdue = "OVERDUE";
        public const string Critical = "CRITICAL";
        public const string Returned = "RETURNED";
    }

    public static class MembershipApplication
    {
        public const string Pending = "PENDING";
        public const string Approved = "APPROVED";
        public const string Rejected = "REJECTED";
        public const string Incomplete = "INCOMPLETE";
    }

    public static class Member
    {
        public const string Active = "ACTIVE";
        public const string Passive = "PASSIVE";
        public const string Suspended = "SUSPENDED";
    }

    public static class Reservation
    {
        public const string Waiting = "WAITING";
        public const string Completed = "COMPLETED";
        public const string Cancelled = "CANCELLED";
        public const string Expired = "EXPIRED";
    }

    public static class Reading
    {
        public const string CurrentlyReading = "CURRENTLY_READING";
        public const string Completed = "COMPLETED";
        public const string Dropped = "DROPPED";
    }

    public static class BookCopy
    {
        public const string Available = "AVAILABLE"; // Rafta
        public const string OnLoan = "ON_LOAN";      // Ödünç Verildi
        public const string InRepair = "IN_REPAIR";  // Tamirde??
        public const string Lost = "LOST";           // Kayıp
    }
}