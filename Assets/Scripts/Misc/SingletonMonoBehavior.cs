/*
 * SingletonMonoBehaviour.cs
 * Author: Joseph Latina
 * Created: February 02, 2024
 * Description: Abstract class that inherits singleton monobehaviour that other classes can inherit from
 */
using UnityEngine;

/// <summary>
/// Abstract class that can't be instatiated but can be inherited by other classes.
/// Class definition allows you to pass in another class type (referenced by T) to inherit from this abstract class
/// </summary>
public abstract class SingletonMonoBehavior<T> : MonoBehaviour where T: MonoBehaviour
{
    // will hold our instance variable
    private static T instance;

    // static variables to be accessed publicly (don't have to instantiate class to access them)
    public static T Instance {

        // getter method to retrieve the instance variable. we can then access any public variable method within this instance
        get {
            return instance;
        }
    }

    /// <summary>
    /// When the class is first instantiated, this awake method will be called first to populate instance variable
    /// Protected means it can be accessed in the inheriting classes and Virtual allows the methods to be overriden by inheriting classes
    /// </summary>
    protected virtual void Awake() {

        // if instance variable is null, then populate it
        if (instance == null) {
            // set the instance variable to the value of the component whose type is T
            instance = this as T;
        } 
        // if there's already an existing instance of this class, then destroy any previous game object instantiated (since there should only be one singleton instance)
        else {
            Destroy(gameObject);
        }
    }

}
