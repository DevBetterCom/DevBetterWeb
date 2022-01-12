using System;
using System.Collections.Generic;
using System.Linq;

namespace DevBetterWeb.Core;

/// <summary>
/// https://code-maze.com/paging-aspnet-core-webapi/
/// </summary>
/// <typeparam name="T"></typeparam>
public class PagedList<T>
{
  public int PageSize { get; set; }
  public int TotalCount { get; set; }
  public List<T> Data { get; set; } = new List<T>();
  public int CurrentPage { get; set; }


  public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
  public bool HasPrevious => CurrentPage > 1;
  public bool HasNext => CurrentPage < TotalPages;

  public bool IsFirstPage => CurrentPage == 1;
  public bool IsLastPage => CurrentPage == TotalPages;
  

  /// <summary>
  /// For deserialization
  /// </summary>
  public PagedList()
  { }

  public PagedList(List<T> items, int count, int pageNumber, int pageSize)
  {
    TotalCount = count;
    PageSize = pageSize;
    CurrentPage = pageNumber;
    Data = items;
  }
}

public static class PagedListExtensions
{
  public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageNumber, int pageSize)
  {
    var count = source.Count();
    var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    return new PagedList<T>(items, count, pageNumber, pageSize);
  }

  public static PagedList<T> ToPagedList<T>(this List<T> source, int count, int pageNumber, int pageSize)
  {
    return new PagedList<T>(source, count, pageNumber, pageSize);
  }
}
