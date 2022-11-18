using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse responce, int currentPage,
            int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage,totalItems,totalPages);
            
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy =  JsonNamingPolicy.CamelCase // תשובה תתחיל מאות קטנה
            };
            
            responce.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));
            responce.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}