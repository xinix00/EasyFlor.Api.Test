using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Test
{
    public class ApiAuthorize
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string Events { get; set; }
    }

    public class ApiListArticleSort
    {
        public IEnumerable<string> ListArticleSorts { get; set; }
    }

    public class ApiDebtorListArticleSortPrice
    {
        public int DebtorId { get; set; }
        public IEnumerable<string> ListArticleSorts { get; set; }
    }
}
