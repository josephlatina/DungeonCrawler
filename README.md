# DungeonCrawler


## Comment Practices
1. **File Header Comment:**
   - **Purpose:** Briefly describes the purpose of the script, author, creation date, and any other relevant information.
   - **Example:**
     ```csharp
     /*
      * PlayerController.cs
      * Author: John Doe
      * Created: January 1, 2023
      * Description: Handles player movement and input.
      */
     ```

2. **Class Comment:**
   - **Purpose:** Describes the purpose of the class, its responsibilities, and any important information about its usage.
   - **Example:**
     ```csharp
     /// <summary>
     /// Manages player input and movement.
     /// </summary>
     public class PlayerController : MonoBehaviour
     {
         // Class implementation...
     }
     ```

3. **Method Comments:**
   - **Purpose:** Describes what each method does, its parameters, return values, and any important details.
   - **Example:**
     ```csharp
     /// <summary>
     /// Moves the player based on input.
     /// </summary>
     /// <param name="direction">The movement direction.</param>
     /// <param name="speed">The movement speed.</param>
     private void Move(Vector3 direction, float speed)
     {
         // Method implementation...
     }
     ```

4. **Variable Comments:**
   - **Purpose:** Provides information about the purpose of a variable, its data type, and any specific details.
   - **Example:**
     ```csharp
     // Represents the player's current health.
     private int playerHealth;
     ```

5. **Public Variable Comments:**
   - **Purpose:** Explains the purpose of public variables and how they should be used.
   - **Example:**
     ```csharp
     /// <summary>
     /// The speed at which the player moves.
     /// </summary>
     public float movementSpeed = 5f;
     ```

6. **Unity Callback Comments:**
   - **Purpose:** Clarifies the purpose of Unity callbacks and when they are triggered.
   - **Example:**
     ```csharp
     /// <summary>
     /// Called when the script is first loaded.
     /// </summary>
     private void Awake()
     {
         // Awake implementation...
     }
     ```

7. **TODO Comments:**
   - **Purpose:** Flags areas of code that need improvement, optimization, or completion.
   - **Example:**
     ```csharp
     // TODO: Refactor this code for better performance.
     ```

8. **Explanation Comments:**
   - **Purpose:** Provides additional context or explanations for complex or non-intuitive code.
   - **Example:**
     ```csharp
     // The following line is used to prevent division by zero.
     float result = (denominator != 0) ? numerator / denominator : 0;
     ```

9. **Remove Commented-Out Code:**
   - **Purpose:** Remove unnecessary commented-out code, as it can clutter the file and confuse developers.
   - **Example:**
     ```csharp
     // int unusedVariable = 42;
     ```

10. **Regular Updates:**
    - **Purpose:** Keep comments up-to-date as code evolves to ensure accuracy.
    - **Example:**
      ```csharp
      /// <summary>
      /// The speed at which the player moves.
      /// </summary>
      public float movementSpeed = 5f;
      ```