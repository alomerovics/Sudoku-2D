using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GridSquare : Selectable,IPointerClickHandler,ISubmitHandler,IPointerExitHandler
{
    public GameObject numberText;
    public List<GameObject> number_notes;
    private bool note_active;
    private int number_ = 0;
    private bool selected = false;
    private int square_index = -1;
    private bool has_default_value_ = false;
    private bool has_wrong_value_ = false;

    public int GetSquareNumber()
    {
        return number_;
    }
    public bool IsCorrectNumberSet()
    {
        return number_ == correct_number_;
    }
    public bool HasWrongValue() { return has_wrong_value_; }
    public void SetHasDefaultValue(bool has_default) { has_default_value_ = has_default; }
    public bool GetHasDefaultValue() { return has_default_value_; }
    private int correct_number_ = 0;
    public bool IsSelected() { return selected; }
    public void SetSquareIndex(int index)
    {
        square_index = index;
    }
    public void SetCorrectNumber(int number)
    {
        correct_number_ = number;
        has_wrong_value_ = false;
    }
    public void SetCorrectNumber()
    {
        number_ = correct_number_;
        SetNoteNumberValue(0);
        DisplayText();
    }

    void Start()
    {
        selected = false;
        note_active = false;
        SetNoteNumberValue(0);
    }
    public List<string> GetSquareNotes()
    {
        List<string> notes = new List<string>();
        foreach (var number in number_notes)
        {
            notes.Add(number.GetComponent<Text>().text);
        }
        return notes;
    }
    private void SetCleatEmptyNotes()
    {
        foreach (var number in number_notes)
        {
            if (number.GetComponent<Text>().text == "0")
                number.GetComponent<Text>().text = " ";
        }
    }
    private void SetNoteSingleNumberValue(int value, bool force_update=false)
    {
        if (note_active == false && force_update == false)
            return;
        if (value <= 0)
            number_notes[value - 1].GetComponent<Text>().text = " ";
        else
        {
            if (number_notes[value - 1].GetComponent<Text>().text == " " || force_update)
                number_notes[value - 1].GetComponent<Text>().text = value.ToString();
            else
                number_notes[value - 1].GetComponent<Text>().text = " ";            
        }
    }
    public void SetGridNotes(List<int> notes)
    {
        foreach (var note in notes)
        {
            SetNoteSingleNumberValue(note, true);
        }
    }
    public void OnNoteActive(bool active)
    {
        note_active = active;
    }
    private void SetNoteNumberValue(int value)
    {
        foreach (var number in number_notes)
        {
            if (value <= 0)
                number.GetComponent<Text>().text = " ";
            else
                number.GetComponent<Text>().text = value.ToString();
        }
    }
    public void DisplayText()
    {
        if (number_<=0)
        {
            numberText.GetComponent<Text>().text = " ";
        }
        else
        {
            numberText.GetComponent<Text>().text = number_.ToString();
        }
    }
    public void SetNumber(int number)
    {
        this.number_ = number;
        DisplayText();


    }

    public void OnPointerClick(PointerEventData eventData)
    {
        selected = true;
        GameEvents.SquareSelectedMethod(square_index);
    }

    public void OnSubmit(BaseEventData eventData)
    {
    }
    private void OnEnable()
    {
        GameEvents.OnUpdateSquareNumber += OnSetNumber;
        GameEvents.OnSquareSelected += OnSquareSelected;
        GameEvents.OnNoteActive += OnNoteActive;
        GameEvents.OnClearNumber += OnClearNumber;
    }
    private void OnDisable()
    {
        GameEvents.OnUpdateSquareNumber -= OnSetNumber;
        GameEvents.OnSquareSelected -= OnSquareSelected;
        GameEvents.OnNoteActive -= OnNoteActive;
        GameEvents.OnClearNumber -= OnClearNumber;
    }
    public void OnClearNumber()
    {
        if (selected && !has_default_value_)
        {
            number_ = 0;
            has_wrong_value_ = false;
            SetSquareColour(Color.white);
            SetNoteNumberValue(0);
            DisplayText();
        }
    }
    public void OnSetNumber(int number)
    {
        if (selected && has_default_value_==false)
        {
            if (note_active == true && has_wrong_value_ == false)
            {
                SetNoteSingleNumberValue(number);
            }
            else if (note_active == false)
            {
                SetNoteNumberValue(0);
                SetNumber(number);
                if (number_ != correct_number_)
                {
                    has_wrong_value_ = true;
                    var colors = this.colors;
                    colors.normalColor = Color.red;
                    this.colors = colors;
                    GameEvents.OnWrongNumberMethod();
                }
                else
                {
                    has_wrong_value_ = false;
                    has_default_value_ = true;
                    var colors = this.colors;
                    colors.normalColor = Color.white;
                    this.colors = colors;
                }
            }
        }
    }
    public void OnSquareSelected(int squareIndex_)
    {
        if (square_index!=squareIndex_)
        {
            selected = false;
        }
    }
    public void SetSquareColour(Color col)
    {
        var colors = this.colors;
        colors.normalColor = col;
        this.colors = colors;
    }
}
