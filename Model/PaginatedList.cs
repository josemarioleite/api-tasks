using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace remarsemanal.Model
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex {get; private set;}
        public int TotalPages {get; private set;}

        public PaginatedList(List<T> itens, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double) pageSize);
            this.AddRange(itens);
        }

        public bool HasPreviousPage{
            get{
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage{
            get{
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize){
            var count = await source.CountAsync();
            var itens = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(itens, count, pageIndex, pageSize);
        }
        public static async Task<PaginatedList<T>> CreateAsync(List<T> source, int pageIndex, int pageSize){
            var count = source.Count;
            var itens = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(itens, count, pageIndex, pageSize);
        }
    }
}