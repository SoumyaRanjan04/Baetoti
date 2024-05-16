using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
	public interface IAccountVisitRepository : IAsyncRepository<AccountVisit>
	{
		Task<AccountVisit> LogAccountVisit(AccountVisit accountVisit);

	}
}
