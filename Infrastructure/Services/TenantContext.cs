using Domain.Interfaces;

namespace Infrastructure.Services
{
    public class TenantContext : ITenantContext
    {
        private Guid? _tenantId;

        public Guid? TenantId => _tenantId;

        public void SetTenantId(Guid tenantId)
        {
            if (_tenantId.HasValue && _tenantId.Value != tenantId)
                throw new InvalidOperationException("TenantId has already been set for this request.");
            _tenantId = tenantId;
        }
    }
}
