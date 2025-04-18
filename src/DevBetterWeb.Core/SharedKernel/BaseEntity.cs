namespace DevBetterWeb.Core.SharedKernel;

// This can be modified to BaseEntity<TId> to support multiple key types (e.g. Guid)
public abstract class BaseEntity : HasDomainEventsBase
{
  public int Id { get; set; }
}
