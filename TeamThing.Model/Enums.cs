namespace TeamThing.Model
{
    public enum TeamUserRole
    {
        Administrator,
        Viewer
    }
    public enum TeamUserStatus
    {
        Approved,
        Pending,
        Denyed
    }

    public enum ThingStatus
    {
        InProgress,
        Completed,
        Deleted,
        Delayed
    }

    public enum ThingAction
    {
        OwnerChanged,
        Completed,
        Deleted,
        Delayed,
        StatusChanged
    }
}