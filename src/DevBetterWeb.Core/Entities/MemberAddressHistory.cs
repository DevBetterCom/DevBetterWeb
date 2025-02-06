using System;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Core.SharedKernel;
using DevBetterWeb.Core.ValueObjects;

namespace DevBetterWeb.Core.Entities;
public class MemberAddressHistory : BaseEntity, IAggregateRoot
{
	public int MemberId { get; private set; }
	public Address? Address { get; private set; }
	public DateTimeOffset UpdatedOn { get; private set; }

	public MemberAddressHistory(int memberId, Address address)
	{
		MemberId = memberId;
		Address = address;
		UpdatedOn = DateTime.UtcNow;
	}

	private MemberAddressHistory()
	{
		// EF
	}
}
