using System;

namespace DevBetterWeb.Core.Exceptions;

public class BookNotFoundException : Exception
{
  public BookNotFoundException(int bookId) : base($"No book found with id {bookId}.")
  {
  }
}
