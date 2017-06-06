using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace ToDoList
{
  public class Task
  {
    private int _id;
    private string _description;
    private string _dueDate;
    private int _categoryId;

    public Task(string Description, string DueDate, int CategoryId, int Id = 0)
    {
      _id = Id;
      _description = Description;
      _dueDate = DueDate;
      _categoryId = CategoryId;
    }
    public override bool Equals(System.Object otherTask)
    {
      if(!(otherTask is Task))
      {
        return false;
      }
      else
      {
        Task newTask = (Task) otherTask;
        bool dueDateEquality = (this.GetDueDate() == newTask.GetDueDate());
        bool descriptionEquality = (this.GetDescription() == newTask.GetDescription());
        bool categoryEquality = (this.GetCategoryId() == newTask.GetCategoryId());
        return (descriptionEquality && dueDateEquality && categoryEquality);
      }
    }
    public int GetCategoryId()
    {
      return _categoryId;
    }
    public void SetCategoryId(int newCategoryId)
    {
      _categoryId = newCategoryId;
    }
    public int GetId()
    {
      return _id;
    }
    public string GetDueDate()
    {
      return _dueDate;
    }

    public void SetDueDate(string newDueDate)
    {
      _dueDate = newDueDate;
    }
    public string GetDescription()
    {
      return _description;
    }
    public void SetDescription(string newDescription)
    {
      _description = newDescription;
    }
    public static List<Task> GetAll()
    {
      List<Task> allTasks = new List<Task>{};

      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM tasks;", conn);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int taskId = rdr.GetInt32(0);
        string taskDueDate = rdr.GetString(2);
        string taskDescription = rdr.GetString(1);
        int taskCategoryId = rdr.GetInt32(3);
        Task newTask = new Task(taskDescription, taskDueDate, taskCategoryId, taskId);
        allTasks.Add(newTask);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allTasks;
    }
    public void Save()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO tasks (description, duedate, category_id) OUTPUT INSERTED.id VALUES (@TaskDescription, @TaskDueDate, @TaskCategoryId);", conn);

      SqlParameter descriptionParameter = new SqlParameter();
      descriptionParameter.ParameterName = "@TaskDescription";
      descriptionParameter.Value = this.GetDescription();

      SqlParameter dueDateParameter = new SqlParameter();
      dueDateParameter.ParameterName = "@TaskDueDate";
      dueDateParameter.Value = this.GetDueDate();

      SqlParameter categoryIdParameter = new SqlParameter();
      categoryIdParameter.ParameterName = "@TaskCategoryId";
      categoryIdParameter.Value = this.GetCategoryId();

      cmd.Parameters.Add(descriptionParameter);
      cmd.Parameters.Add(dueDateParameter);
      cmd.Parameters.Add(categoryIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }
    public static Task Find(int id)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM tasks WHERE id = @TaskId;", conn);
      SqlParameter taskIdParameter = new SqlParameter();
      taskIdParameter.ParameterName = "@TaskId";
      taskIdParameter.Value = id.ToString();
      cmd.Parameters.Add(taskIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int foundTaskId = 0;
      string foundTaskDescription = null;
      string foundDueDate = null;
      int foundTaskCategoryId = 0;
      while(rdr.Read())
      {
        foundTaskId = rdr.GetInt32(0);
        foundTaskDescription = rdr.GetString(1);
        foundDueDate = rdr.GetString(2);
        foundTaskCategoryId = rdr.GetInt32(3);
      }
      Task foundTask = new Task(foundTaskDescription, foundDueDate, foundTaskCategoryId, foundTaskId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      Console.WriteLine(foundTask.GetDueDate());
      return foundTask;
    }
    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM tasks;", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }
  }
}
