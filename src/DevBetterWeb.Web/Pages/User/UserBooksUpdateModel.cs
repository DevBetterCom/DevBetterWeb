﻿using DevBetterWeb.Core.Entities;
using DevBetterWeb.Core.Interfaces;
using DevBetterWeb.Infrastructure.Data;
using DevBetterWeb.Infrastructure.DomainEvents;
using DevBetterWeb.Web.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DevBetterWeb.Web.Pages.User
{
    public class UserBooksUpdateModel
    {

        public List<Book> BooksRead { get; set; } = new List<Book>();
        public int? AddedBook { get; set; }
        public int? RemovedBook { get; set; }

        public UserBooksUpdateModel()
        {

        }

        public UserBooksUpdateModel(Member member)
        {

            BooksRead = member.BooksRead!;
            
        }

        public bool HasReadBook(Book book)
        {
            if (BooksRead == null)
            {
                return false;
            }

            foreach (Book Book in BooksRead)
            {
                if (Book != null && Book.Equals(book))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
