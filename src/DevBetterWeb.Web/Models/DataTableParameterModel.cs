namespace DevBetterWeb.Web.Models
{
  public class DataTableParameterModel
  {
    /// <summary>
    /// Draw counter. This is used by DataTables to ensure that the Ajax returns from 
    /// server-side processing requests are drawn in sequence by DataTables 
    /// </summary>
    public string Draw { get; set; }

    /// <summary>
    /// Paging first record indicator. This is the start point in the current data set 
    /// (0 index based - i.e. 0 is the first record)
    /// </summary>
    public string Start { get; set; }

    /// <summary>
    /// Number of records that the table can display in the current draw. It is expected
    /// that the number of records returned will be equal to this number, unless the 
    /// server has fewer records to return. Note that this can be -1 to indicate that 
    /// all records should be returned (although that negates any benefits of 
    /// server-side processing!)
    /// </summary>
    public string Length { get; set; }

  }
}
