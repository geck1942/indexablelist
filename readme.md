The IndexableList works as a classic Generic.List, but works with elements indexed by a unique key. The IndexableList allows elements to be found by a reference or by their Key.

The IndexableList depends on two types: TKey for the tye of the elements unique key, and TValue the type of our elements. It also states that TKey must implement the interface IEquatable, which means the function Equals(). This is already the case for String or Guid. As well, the elements of type TValue must implement the interface IIndexable:

```
/// <summary>
/// The class that depends on this Interface must return a getter (Key) that return a unique key from which an instance of this object will be indexed.
/// </summary>
/// <typeparam name="TKey">Type of the unique key of the object to index</typeparam>
public interface IIndexable<TKey> 
	where TKey : IEquatable<TKey>
{
	/// <summary>
	/// How to read the key from this IIndexable item.
	/// </summary>
	TKey Key { get; }
}
```

An exemple of implementation of this interface could look like:
```
namespace Polycorne.SiliconCity.Mechanics.Navigation.Waypoints
{
    public class Waypoint: IEquatable<Waypoint>, IIndexable<Guid>
    {
        [DataMember]
        public Guid UID { get; private set; }

        // Implementation of IIndexable's Key
        public Guid Key => this.UID;
    }
}
```
Here's how nos the IndexableList can be used:
```
public IndexableList<Guid, Waypoint> Path = new IndexableList<Guid, Waypoint>();

Path.Add(origin);
Path.AddRange(nextwaypoints);
Path.Add(destination);

if(Path.Contains(destination))
{
    Path.Remove(destination);
}
```
The main idea of this class, is to very quickly run the *Contains()* operation. this is why the class has both an internal list and a dictionary. It also come with two optional settings, IndexNullValues and AllowNonUniqueKey that can be respectively turned on or off.

A more detailed article can be found here: 