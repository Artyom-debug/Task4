using Microsoft.AspNetCore.Identity;

namespace Task4.Models;

public sealed class ApplicationUser : IdentityUser
{
    public Status Status { get; private set; }

    public DateTime LastLoginTime { get; private set; }

    public void Block()
    {
        this.Status = Status.blocked;
    }

    public void VerifyUser()
    {
        this.Status = Status.active;
    }

    public void Unblock(Status status)
    {
        this.Status = status;
    }

    public void SetLoginTime(DateTime time)
    {
        this.LastLoginTime = time;
    }
}

public enum Status
{
    unverified, active, blocked 
}
