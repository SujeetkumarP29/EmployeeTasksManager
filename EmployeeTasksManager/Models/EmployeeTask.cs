﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EmployeeTasksManager.Models;

public partial class EmployeeTask
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime? DueDate { get; set; }

    public string Status { get; set; }

    public string EmployeeId { get; set; }

    public DateTime TaskCreatedDate { get; set; }

    public virtual Employee Employee { get; set; }
}