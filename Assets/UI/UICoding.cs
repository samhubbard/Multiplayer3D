using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UICoding : MonoBehaviour
{
    //UI Document Object.
    public UIDocument doc;
    //The very first visual element object, also known as the 'root'.
    private VisualElement root;
    //The blue UI button.
    private Button clickButton;
    //The label below the button.
    private Label incrementCount;
    //A counter.
    private int counter;
    
    void Start()
    {
        //Initialize counter to start at 0.
        counter = 0;
        //Initialize root.
        root = doc.rootVisualElement;
        //Initialize the button object by connecting it to the button in the layout.
        //The Q function with whatever type of element it is in the <> braces allows you to find and connect
        //objects from the layout to code.
        clickButton = root.Q<Button>("button-one");
        //Same goes for the label.
        incrementCount = root.Q<Label>("label-one");

        //Tie the ClickTheButton() function to execute when the button is clocked.
        clickButton.clicked += ClickTheButton;
        //Change the label text to "0" at start.
        incrementCount.text = counter.ToString();
    }

    void ClickTheButton()
    {
        //Add 1 to the counter when the button is clicked.
        counter++;
        //Change the label text to be up-to-date.
        incrementCount.text = counter.ToString();
    }

}