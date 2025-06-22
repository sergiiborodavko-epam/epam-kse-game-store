namespace EpamKse.GameStore.Services.Services.License;
using Domain.Entities;

public interface ILicenseBuilder
{
    byte[] Build(License license);
}