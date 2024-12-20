using LiteDB;
using System.IO;
using System.Linq;

namespace Global;
public class LiteDBProps
{
    public class Prop
    {
        public long Id {
            get; set;
        }
        public string Name {
            get; set;
        }
        public object Data {
            get; set;
        }
    }
    private string filePath = null;
    public LiteDBProps(DirectoryInfo di)
    {
        this.filePath = Path.Combine(di.FullName, "properties.litedb");
        Dirs.PrepareForFile(this.filePath);
        using (var connection = new LiteDatabase(new ConnectionString(this.filePath)
        {
            Connection = ConnectionType.Shared
        }))
        {
            var collection = connection.GetCollection<Prop>("properties");
            // Nameをユニークインデックスにする
            collection.EnsureIndex(x => x.Name, true);
        }
    }
    public LiteDBProps(string orgName, string appNam) : this(new DirectoryInfo(Dirs.ProfilePath(orgName, appNam)))
    {
    }
    public EasyObject Get(string name)
    {
        using (var connection = new LiteDatabase(new ConnectionString(this.filePath)
        {
            Connection = ConnectionType.Shared
        }))
        {
            connection.BeginTrans();
            var collection = connection.GetCollection<Prop>("properties");
            var result = collection.Find(x => x.Name == name).FirstOrDefault();
            connection.Commit();
            if (result == null) return null;
            return EasyObject.FromObject(result.Data);
        }
    }
    public void Put(string name, dynamic? data)
    {
        if (data is EasyObject) data = ((EasyObject)data).ToObject();
        using (var connection = new LiteDatabase(new ConnectionString(this.filePath)
        {
            Connection = ConnectionType.Shared
        }))
        {
            connection.BeginTrans();
            var collection = connection.GetCollection<Prop>("properties");
            var result = collection.Find(x => x.Name == name).FirstOrDefault();
            if (result == null)
            {
                result = new Prop {
                    Name = name, Data = data
                };
                collection.Insert(result);
                connection.Commit();
                return;
            }
            result.Data = data;
            collection.Update(result);
            connection.Commit();
        }
    }
    public void Delete(string name)
    {
        using (var connection = new LiteDatabase(new ConnectionString(this.filePath)
        {
            Connection = ConnectionType.Shared
        }))
        {
            connection.BeginTrans();
            var collection = connection.GetCollection<Prop>("properties");
            var result = collection.Find(x => x.Name == name).FirstOrDefault();
            if (result == null)
            {
                connection.Commit();
                return;
            }
            collection.Delete(result.Id);
            connection.Commit();
        }
    }
    public void DeleteAll()
    {
        using (var connection = new LiteDatabase(new ConnectionString(this.filePath)
        {
            Connection = ConnectionType.Shared
        }))
        {
            connection.BeginTrans();
            var collection = connection.GetCollection<Prop>("properties");
            collection.DeleteAll();
            connection.Commit();
        }
    }
}
