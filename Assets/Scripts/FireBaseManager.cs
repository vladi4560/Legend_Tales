using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity;
using UnityEngine.Events;
using System.Threading.Tasks;
using System;

public class FireBaseManager : MonoBehaviour
{
    public static FireBaseManager instance { get; private set; }

    private FirebaseApp app;
    private DatabaseReference reference;

    public bool IsFirebaseInitialized { get; private set; } = false;

    public string userAddress;

    public async void Awake()
    {
        Debug.Log("FirebaseManager: Awake() started.");  // Log message indicating Awake() was called
        // Check if an Instance already exists
        if (instance == null)
        {
            // If not, set this as the Instance
            instance = this;
            DontDestroyOnLoad(gameObject); // Ensures that this object isn't destroyed when loading new scenes


            await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    // Initialize Firebase
                    app = FirebaseApp.DefaultInstance;

                    // Get a reference to the Firebase Realtime Database
                    //reference = FirebaseDatabase.DefaultInstance.RootReference;
                    reference = FirebaseDatabase.GetInstance("https://lagened-tales-default-rtdb.europe-west1.firebasedatabase.app/").RootReference;

                    IsFirebaseInitialized = true;
                    userAddress = "";
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }
        else
        {
            // If an Instance already exists, destroy this GameObject to ensure there's only ever one FirebaseManager
            Destroy(gameObject);
        }
    }
    public void SaveNewUser(string userAddress, string userName)
    {
        this.userAddress = userAddress;
        if (app == null || reference == null)
        {
            Debug.LogError("Firebase has not been initialized yet. Make sure FirebaseManager.Awake has been called and finished before calling SaveNewUser.");
            return;
        }

        // Use userAddress to generate a deterministic, unique ID

        DatabaseReference userRef = reference.Child("Users").Child(userAddress);


        userRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                Debug.LogError($"Failed to check user: {task.Exception}");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // If the user does not exist, add the new user
                if (!snapshot.Exists)
                {
                    Debug.Log($"Saving new user with address: {userAddress}, name: {userName}, and gLTT: 100");

                    userRef.Child("address").SetValueAsync(userAddress);
                    userRef.Child("name").SetValueAsync(userName);
                    userRef.Child("gLTT").SetValueAsync(100);
                }
                else
                {
                    Debug.Log($"User with address {userAddress} already exists.");

                }
            }
        });
    }

    public async Task AddItemToIdList(int itemId)
    {

        // Load the existing list
        var dataSnapshot = await reference.Child("Users").Child(userAddress).Child("myIds").GetValueAsync();

        // Create a new list or use the existing one
        List<int> currentList = new List<int>();
        if (dataSnapshot.Exists)
        {
            var existingData = JsonUtility.FromJson<Wrapper>(dataSnapshot.GetRawJsonValue());
            currentList = existingData.List;
        }

        // Add the new item
        currentList.Add(itemId);

        // Update the list back to Firebase
        string json = JsonUtility.ToJson(new Wrapper { List = currentList });
        await reference.Child("Users").Child(userAddress).Child("myIds").SetRawJsonValueAsync(json);
    }

    public async Task RemoveIdFromList(int itemId)
    {

        var dataSnapshot = await reference.Child("Users").Child(userAddress).Child("myIds").GetValueAsync();

        // Create a new list or use the existing one
        List<int> currentList = new List<int>();
        if (dataSnapshot.Exists)
        {
            var existingData = JsonUtility.FromJson<Wrapper>(dataSnapshot.GetRawJsonValue());
            currentList = existingData.List;
        }

        // Check if the item exists in the list
        if (currentList.Contains(itemId))
        {
            // Remove the item
            currentList.Remove(itemId);

            // Update the list back to Firebase
            string json = JsonUtility.ToJson(new Wrapper { List = currentList });
            await reference.Child("Users").Child(userAddress).Child("myIds").SetRawJsonValueAsync(json);
        }
        else
        {
            Debug.Log($"Item with id {itemId} not found in the list.");
        }
        // //var dataSnapshot =
        // string json = JsonUtility.ToJson(new Wrapper { List = myIds });

        // await reference.Child("Users").Child(userId).Child("myIds").SetValueAsync(json);




    }

    public async Task<List<int>> LoadIntList()
    {
        DatabaseReference userRef = reference.Child("Users").Child(userAddress);


        var dataSnapshot = await userRef.Child("myIds").GetValueAsync();
        if (dataSnapshot.Exists)
        {
            return JsonUtility.FromJson<Wrapper>(dataSnapshot.GetRawJsonValue()).List;
        }
        return new List<int>();
    }

    public void AddNewListedNFT(ListedNFT listedNFT)
    {
        string json = JsonUtility.ToJson(listedNFT);
        reference.Child("listedNFTs").Push().SetRawJsonValueAsync(json);
    }
    public async Task initListedNFTList()
    {
        var dataSnapshot = await reference.Child("listedNFTs").GetValueAsync();
        foreach (var child in dataSnapshot.Children)
        {
            string json = child.GetRawJsonValue();
            ListedNFT listedNFT = JsonUtility.FromJson<ListedNFT>(json);
            CardDataBase.instance.listed.Add(listedNFT);
        }
    }
    public async Task RemoveFromListingByTokenId(int tokenId) // not working
    {
        var dataSnapshot = await reference.Child("listedNFTs").GetValueAsync();
        foreach (var child in dataSnapshot.Children)
        {
            ListedNFT listedNFT = JsonUtility.FromJson<ListedNFT>(child.GetRawJsonValue());
            if (listedNFT.tokenId == tokenId)
            {
                await reference.Child("listedNFTs").Child(child.Key).RemoveValueAsync();
                break;
            }
        }
    }
    public async Task AddIdToName(int key, string value)
    {
        await reference.Child("idToName").Child(key.ToString()).SetValueAsync(value);
    }
    public async Task<Dictionary<int, string>> LoadIdToNameDictionary()
    {
        var dataSnapshot = await reference.Child("idToName").GetValueAsync();
        var idToName = new Dictionary<int, string>();
        foreach (var child in dataSnapshot.Children)
        {
            idToName[int.Parse(child.Key)] = child.Value.ToString();
            Debug.Log("line 104 " + child.Value.ToString());
        }
        return idToName;
    }
    public async Task<string> LoadGLTT()
    {
        DatabaseReference userRef = reference.Child("Users").Child(userAddress);
        var dataSnapshot = await userRef.Child("gLTT").GetValueAsync();
        if (dataSnapshot.Exists)
            return dataSnapshot.Value.ToString();
        else
        {
            Debug.LogError("No gLTT value found for user: " + userAddress);
            return "";
        }
    }
    public void updateGLTT(int gltt)
    {
        DatabaseReference userRef = reference.Child("Users").Child(userAddress);


        var dataSnapshot = userRef.Child("gLTT").SetValueAsync(gltt);
    }

    public async Task<string> getListedAddress(int tokenId)
    {
        var dataSnapshot = await reference.Child("listedNFTs").GetValueAsync();
        foreach (var child in dataSnapshot.Children)
        {
            ListedNFT listedNFT = JsonUtility.FromJson<ListedNFT>(child.GetRawJsonValue());
            if (listedNFT.tokenId == tokenId)
            {
                return listedNFT.sellerAddress;
            }
        }
        return "Seller not found";
    }

    void Update()
    {

    }
}

[Serializable]
public class Wrapper
{
    public List<int> List;
}