using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ToDoList.Models
{
  public class Category
  {
    private static List<Category> _instances = new List<Category> {};
    private string _name;
    private int _id;

    public Category(string categoryName, int id = 0)
    {
      _name = categoryName;
      _id = id;
    }

    public string GetName()
    {
      return _name;
    }

    public int GetId()
    {
      return _id;
    }


  public static void ClearAll()
  {
    MySqlConnection conn = DB.Connection();
    conn.Open();
    var cmd = conn.CreateCommand() as MySqlCommand;
    cmd.CommandText = @"DELETE FROM categories;";
    cmd.ExecuteNonQuery();
    conn.Close();
    if (conn != null)
    {
      conn.Dispose();
    }
  }

  public static List<Category> GetAll()
  {
    List<Category> allCategories = new List<Category> {};
    MySqlConnection conn = DB.Connection();
    conn.Open();
    var cmd = conn.CreateCommand() as MySqlCommand;
    cmd.CommandText = @"SELECT * FROM categories;";
    var rdr = cmd.ExecuteReader() as MySqlDataReader;
    while(rdr.Read())
    {
      int CategoryId = rdr.GetInt32(0);
      string CategoryName = rdr.GetString(1);
      Category newCategory = new Category(CategoryName, CategoryId); // <-- This line now has two arguments
      allCategories.Add(newCategory);
    }
    conn.Close();
    if (conn != null)
    {
      conn.Dispose();
    }
    return allCategories;
  }


public static Category Find(int id)
  {
    MySqlConnection conn = DB.Connection();
    conn.Open();
    var cmd = conn.CreateCommand() as MySqlCommand;
    cmd.CommandText = @"SELECT * FROM categories WHERE id = (@searchId);";
    MySqlParameter searchId = new MySqlParameter();
    searchId.ParameterName = "@searchId";
    searchId.Value = id;
    cmd.Parameters.Add(searchId);
    var rdr = cmd.ExecuteReader() as MySqlDataReader;
    int CategoryId = 0;
    string CategoryName = "";
    while(rdr.Read())
    {
      CategoryId = rdr.GetInt32(0);
      CategoryName = rdr.GetString(1);
    }
    Category newCategory = new Category(CategoryName, CategoryId);
    conn.Close();
    if (conn != null)
    {
      conn.Dispose();
    }
    return newCategory;
  }


public List<Item> GetItems()
    {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = 
        @"SELECT items.* FROM categories
        JOIN categories_items ON (categories.id = categories_items.category_id)
        JOIN items ON (categories_items.item_id = items.id)
        WHERE categories.id = @CategoryId;";
        MySqlParameter categoryIdParameter = new MySqlParameter();
        categoryIdParameter.ParameterName = "@CategoryId";
        categoryIdParameter.Value = _id;
        cmd.Parameters.Add(categoryIdParameter);
        MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
        List<Item> items = new List<Item>{};
        while(rdr.Read())
        {
          int itemId = rdr.GetInt32(0);
          string itemDescription = rdr.GetString(1);
          Item newItem = new Item(itemDescription, itemId);
          items.Add(newItem);
        }
        conn.Close();
        if (conn != null)
        {
          conn.Dispose();
        }
        return items;
    }



    public override bool Equals(System.Object otherCategory)
    {
      if (!(otherCategory is Category))
      {
        return false;
      }
      else
      {
        Category newCategory = (Category) otherCategory;
        bool idEquality = this.GetId().Equals(newCategory.GetId());
        bool nameEquality = this.GetName().Equals(newCategory.GetName());
        return (idEquality && nameEquality);
      }
    }

  public void Save()
  {
    MySqlConnection conn = DB.Connection();
    conn.Open();
    var cmd = conn.CreateCommand() as MySqlCommand;
    cmd.CommandText = @"INSERT INTO categories (name) VALUES (@name);";
    MySqlParameter name = new MySqlParameter();
    name.ParameterName = "@name";
    name.Value = this._name;
    cmd.Parameters.Add(name);
    cmd.ExecuteNonQuery();
    _id = (int) cmd.LastInsertedId; // <-- This line is new!
    conn.Close();
    if (conn != null)
    {
      conn.Dispose();
    }
  }

     public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = new MySqlCommand("DELETE FROM categories WHERE id = @CategoryId; DELETE FROM categories_items WHERE category_id = @CategoryId;", conn);
      MySqlParameter categoryIdParameter = new MySqlParameter();
      categoryIdParameter.ParameterName = "@CategoryId";
      categoryIdParameter.Value = this.GetId();
      cmd.Parameters.Add(categoryIdParameter);
      cmd.ExecuteNonQuery();
      if (conn != null)
      {
        conn.Close();
      }
    }


    public void AddItem(Item newItem)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO categories_items (category_id, item_id) VALUES (@CategoryId, @ItemId);";
      MySqlParameter category_id = new MySqlParameter();
      category_id.ParameterName = "@CategoryId";
      category_id.Value = _id;
      cmd.Parameters.Add(category_id);
      MySqlParameter item_id = new MySqlParameter();
      item_id.ParameterName = "@ItemId";
      item_id.Value = newItem.GetId();
      cmd.Parameters.Add(item_id);
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }




  }
}
