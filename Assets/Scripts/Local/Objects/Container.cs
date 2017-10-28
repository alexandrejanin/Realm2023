using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class Container : Interactable, IEnumerable<Item> {
	protected virtual List<Item> Items { get; }
	public readonly int maxSize;

	public int FreeSpace => maxSize - TakenSpace;
	public int TakenSpace => Items.Sum(item => item.size);

	public int ItemCount => Items.Count;

	public bool CanAddItem(Item item) => FreeSpace >= item.size && !Contains(item);

	public override string Name { get; }

	protected Container(string name, Location location, Coord position, int maxSize) : base(location, position) {
		Name = name;
		Items = new List<Item>();
		this.maxSize = maxSize;
	}

	public virtual void Update() {
		foreach (Item item in Items) {
			item.position = position;
		}
	}

	public virtual bool AddItem(Item item) {
		if (!CanAddItem(item)) return false;

		item.container?.RemoveItem(item);

		Items.Add(item);
		item.container = this;
		return true;
	}

	public virtual void RemoveItem(Item item) {
		if (!Contains(item)) throw new ArgumentException("Item not in container");

		Items.Remove(item);
		item.container = null;
	}

	public bool Contains(Item item) => Items.Contains(item);

	public override List<Interaction> GetInteractions(Character character) => GetBasicInteractions(character);

	protected override string InspectText() => Name;

	public virtual IEnumerator<Item> GetEnumerator() => Items.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}