namespace Library.Entity.Constants;

public static class Statuses
{
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
}