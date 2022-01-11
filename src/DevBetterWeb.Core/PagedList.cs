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
  public int CurrentPage { get; set; }
  public int TotalPages { get; set; }
  public int PageSize { get; set; }
  public int TotalCount { get; set; }
  public bool HasPrevious => CurrentPage > 1;
  public bool HasNext => CurrentPage < TotalPages;
  public List<T> Data { get; set; } = new List<T>();

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
    TotalPages = (int)Math.Ceiling(count / (double)pageSize);
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
