using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionData {

	public SortedDictionary<int, int> ballSections;

	public SectionData()
	{
		ballSections = new SortedDictionary<int, int>();
		ballSections.Add(int.MaxValue, 0);
	}

	public int GetSectionKey(int front)
	{
		int key = int.MaxValue;

		foreach(KeyValuePair<int, int> entry in ballSections)
		{
			if (front >= entry.Value && front <= entry.Key)
				key =  entry.Key;
		}

		return key;
	}

	public void OnAddModifySections(int atIndex)
	{
		List<KeyValuePair<int, int>> modSectionList = new List<KeyValuePair<int, int>>();
		int sectionKey = GetSectionKey(atIndex);

		int sectionKeyVal;
		ballSections.TryGetValue(sectionKey, out sectionKeyVal);

		// Range will be 1 only when a new ball is added
		// else it will be >= 3 when balls are to be removed
		if (sectionKey != int.MaxValue) {
			int newSectionKey = sectionKey + 1;
			ballSections.Add(newSectionKey, sectionKeyVal);
			ballSections.Remove(sectionKey);

			// Get the keys which are to be updated
			// Since changing the value in the loop itself will add new entries which are always greater than the current one
			// There by looping it infinitely and incorrect
			foreach (KeyValuePair<int, int> entry in ballSections)
			{
				if (entry.Key > newSectionKey)
					modSectionList.Add(entry);
			}

			// Update the sections seperatly to avoid wrong calulation when done in above loop
			// For all the value other than the end of chain modifiy both their key and value
			// For the end of chain section only update the value i.e its front
			foreach(KeyValuePair<int, int> entry in modSectionList)
			{
				if (entry.Key != int.MaxValue)
				{
					if (entry.Value == 0)
						ballSections.Add(entry.Key + 1, entry.Value);
					else
						ballSections.Add(entry.Key + 1, entry.Value + 1);

					ballSections.Remove(entry.Key);
				}
				else
					ballSections[entry.Key] = entry.Value + 1;
			}
		}
	}

	public void DeleteEntireSection(int atIndex, int range, int sectionKey, int ballListCount)
	{
		List<KeyValuePair<int, int>> modSectionList = new List<KeyValuePair<int, int>>();

		// If section is last but one i.e before the moving section
		if (atIndex + range != ballListCount + range)
		{
			Debug.Log("Entire: ");
			// modify the back values for the immediate next section.
			// modify the back and front values for other higher ones
			ballSections.Remove(sectionKey);

			foreach (KeyValuePair<int, int> entry in ballSections)
			{
				if (entry.Key > sectionKey)
					modSectionList.Add(entry);
			}

			ballSections.Add(modSectionList[0].Key - range, atIndex);
			ballSections.Remove(modSectionList[0].Key);
			modSectionList.RemoveAt(0);

			foreach(KeyValuePair<int, int> entry in modSectionList)
			{
				if (entry.Key != int.MaxValue)
				{
					ballSections.Add(entry.Key - range, entry.Value - range);
					ballSections.Remove(entry.Key);
				}
				else
					ballSections[entry.Key] = entry.Value - range;
			}
		}
		else
		{
			KeyValuePair<int, int> getLastButOne = new KeyValuePair<int, int>();

			if (ballSections.Count > 1)
			{
				foreach (KeyValuePair<int, int> entry in ballSections)
				{
					if (entry.Key < sectionKey)
						getLastButOne = entry;
				}

				ballSections.Remove(getLastButOne.Key);
				ballSections[int.MaxValue] = getLastButOne.Value;
			}
		}
	}

	public void DeletePartialSection(int atIndex, int range, int sectionKey, int sectionKeyVal, int ballListCount)
	{
		List<KeyValuePair<int, int>> modSectionList = new List<KeyValuePair<int, int>>();

		// Handle cases
		// when delection takes place at the front or back of the section
		int end = sectionKey == int.MaxValue? ballListCount + range - 1: sectionKey;
		if (atIndex == sectionKeyVal || atIndex + range - 1 == end)
		{
			Debug.Log("Partial: Front/back");
			if (sectionKey == int.MaxValue)
				return;

			int newSectionKey = sectionKey - range;
			ballSections.Add(newSectionKey, sectionKeyVal);
			ballSections.Remove(sectionKey);

			foreach (KeyValuePair<int, int> entry in ballSections)
			{
				if (entry.Key > newSectionKey)
					modSectionList.Add(entry);
			}
		}
		// when delection takes place in middle of the section which creates a new section
		else
		{
			Debug.Log("Partial: Middle");
			// new section front
			int newSectionKey = atIndex - 1;
			ballSections.Add(newSectionKey, sectionKeyVal);

			if (sectionKey != int.MaxValue)
			{
				int nextSectionKey = sectionKey - range;

				ballSections.Remove(sectionKey);

				// new section back
				ballSections.Add( nextSectionKey, atIndex);

				foreach (KeyValuePair<int, int> entry in ballSections)
				{
					if (entry.Key > nextSectionKey)
						modSectionList.Add(entry);
				}
			}
			else
			{
				ballSections[int.MaxValue] = atIndex;
				return;
			}
		}

		// Sort keys just to avoid bug due if a lower key is modified before higher key
		modSectionList.Sort((entryA, entryB) => entryA.Key.CompareTo(entryB.Key));


		// Get the keys to be changed and modify them
		foreach(KeyValuePair<int, int> entry in modSectionList)
		{
			if (entry.Key != int.MaxValue)
			{
				if (entry.Value == 0)
					ballSections.Add(entry.Key - range, entry.Value);
				else
					ballSections.Add(entry.Key - range, entry.Value - range);

				ballSections.Remove(entry.Key);
			}
			else
				ballSections[entry.Key] = entry.Value - range;
		}
	}
}
