namespace Aigang.Policies.Domain
{
    public enum PolicyStatus
    {
        NotSet = 0,
        Draft = 1,
        Paid = 2,
        Claimable = 3,
        Paidout = 4,
        Canceled = 5,
        Finished  = 8
    }
}