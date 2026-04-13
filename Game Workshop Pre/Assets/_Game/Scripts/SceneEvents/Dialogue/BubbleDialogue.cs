using System;
using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class BubbleDialogue : MonoBehaviour
{
    [SerializeField, Min(1)] private int _bubbleWidth = 1;
    [SerializeField, Min(1)] private int _bubbleHeight = 1;
    [SerializeField, Range(0f,1f)] private float _bubbleTailPosition = 50f;
    [SerializeField] private bool _flipTailHorizontally;
    [SerializeField] private bool _flipTailVertically;
    [SerializeField] private int _maxBubbleWidth = 100;
    [SerializeField] private Vector2 _parentPosition = Vector2.zero;
    [SerializeField] private Vector2  _bubbleOffset = Vector2.zero;

    [Header("Text Printing")]
    [SerializeField] private bool _instantText = false;
    [SerializeField] private float _textSpeed = 0.05f;
    [SerializeField] private float _lineLingerTime = 2f;

    [Header("Structure")]
    [SerializeField] private Image _bubbleCenter;
    [SerializeField] private Image _bubbleCornerTL;
    [SerializeField] private Image _bubbleCornerTR;
    [SerializeField] private Image _bubbleCornerBL;
    [SerializeField] private Image _bubbleCornerBR;
    [SerializeField] private Image _bubbleSideTop;
    [SerializeField] private Image _bubbleSideBottom;
    [SerializeField] private Image _bubbleSideLeft;
    [SerializeField] private Image _bubbleSideRight;
    [SerializeField] private Image _bubbleTail;
    [SerializeField] private TMP_Text _textField;


    //Coroutine for DisplayLine method
    private Coroutine _displayLineCoroutine;

    public static BubbleDialogue TryCreate(GameObject parentObject, Vector2Int size, Vector2 offset)
    {
        BubbleDialogue newDialogue = null;
        WorldCanvasManager wcm = WorldCanvasManager.Instance;
        if (wcm != null)
        {
            GameObject prefab = wcm.BubbleDialoguePrefab;
            GameObject dialogueObject = Instantiate(prefab, wcm.transform);
            newDialogue = dialogueObject.GetComponent<BubbleDialogue>();
            newDialogue.Setup(parentObject, size, offset);
        }

        return newDialogue;
    }

    private void OnValidate()
    {
        UpdateBubbleSize();
    }

    public void Setup(GameObject parentObject, Vector2Int size, Vector2 offset)
    {
        gameObject.name = parentObject.name + ":BubbleDialogue";
        _bubbleWidth = size.x;
        _bubbleHeight = size.y;
        _parentPosition = parentObject.transform.position;
        _bubbleOffset = offset;
        UpdateBubbleSize();
        UpdateBubblePosition();
    }

    public void Write(string text, Action onComplete = null)
    {
        if (_displayLineCoroutine != null) StopCoroutine(_displayLineCoroutine);
        _displayLineCoroutine = StartCoroutine(DisplayLine(text, onComplete));
    }

    // Cancels current line being written.
    public void CancelWrite()
    {
        if (_displayLineCoroutine != null) StopCoroutine(_displayLineCoroutine);
        _textField.text = "";
    }

    public void Close()
    {
        CancelWrite();
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void End()
    {
        CancelWrite();
        Destroy(gameObject);
    }


    // Display lines of text character by character
    private IEnumerator DisplayLine(string line, Action onComplete) {
        _textField.text = line;
        
        Vector2 size = _textField.GetPreferredValues(_maxBubbleWidth - 4, Mathf.Infinity);
        _bubbleWidth = Mathf.Min(Mathf.CeilToInt(size.x), _maxBubbleWidth) - 4;
        _bubbleHeight = Mathf.CeilToInt(size.y) - 4;

        UpdateBubbleSize();
        UpdateBubblePosition();

        _textField.maxVisibleCharacters = 0;
        yield return new WaitForEndOfFrame();

        bool isAddingRichTextTag = false;
        foreach (char c in line) 
        {
            //force text to print instantly
            if (_instantText) {
                _textField.maxVisibleCharacters = line.Length;
                break;
            }
            //check for adding a rich text tag
            if (c == '<' || isAddingRichTextTag) {
                isAddingRichTextTag = true;
                if (c == '>') {
                    isAddingRichTextTag = false;
                }
            } else { //not adding rich text tag, print text normally
                _textField.maxVisibleCharacters++;
                yield return new WaitForSeconds(_textSpeed);
            }
        }

        yield return new WaitForSeconds(_lineLingerTime);
        onComplete?.Invoke();
    }

    [ContextMenu("Update Bubble")]
    private void UpdateBubbleSize()
    {
        float w = _bubbleWidth;
        float h = _bubbleHeight;

        // Center - Change scale
        _bubbleCenter.rectTransform.localScale = new Vector2(w, h);

        // Sides - Change top and bottom width for w, change left and right width for h
        _bubbleSideTop.rectTransform.localScale = new Vector2(w, _bubbleSideTop.rectTransform.localScale.y);
        _bubbleSideBottom.rectTransform.localScale = new Vector2(w, _bubbleSideBottom.rectTransform.localScale.y);
        _bubbleSideLeft.rectTransform.localScale = new Vector2(h, _bubbleSideLeft.rectTransform.localScale.y);
        _bubbleSideRight.rectTransform.localScale = new Vector2(h, _bubbleSideRight.rectTransform.localScale.y);

        //Positions are (dimension * 0.5) +3. Bottom and Left are negative. Top and Bottom use height to modify y, Left and Right use width to modify x
        _bubbleSideTop.rectTransform.localPosition = new Vector2(0, (0.5f * h)+ 3);
        _bubbleSideBottom.rectTransform.localPosition = new Vector2(0, -(0.5f * h)- 3);
        _bubbleSideLeft.rectTransform.localPosition = new Vector2(-(0.5f * w)- 3, 0);
        _bubbleSideRight.rectTransform.localPosition = new Vector2((0.5f * w)+ 3, 0);

        // Corners - Move positions to be at (2.5 + w * 0.5, 2.5 + h * 0.5). Account for signs depending on direction
        // TL (-, +)
        // TR (+, +)
        // BL (-, -)
        // BR (+, -)
        _bubbleCornerTL.rectTransform.localPosition = new Vector2(-(2.5f + w * 0.5f), 2.5f + h * 0.5f);
        _bubbleCornerTR.rectTransform.localPosition = new Vector2(2.5f + w * 0.5f, 2.5f + h * 0.5f);
        _bubbleCornerBL.rectTransform.localPosition = new Vector2(-(2.5f + w * 0.5f), -(2.5f + h * 0.5f));
        _bubbleCornerBR.rectTransform.localPosition = new Vector2(2.5f + w * 0.5f, -(2.5f + h * 0.5f));


        // Tail
        float tailMinX = 0f;
        float tailMaxX = -1f;
        float tailY = -6f;

        // Move tail position based on height
        tailY -= h * 0.5f;
        // Move tail min and max based on width 
        tailMinX -= w * 0.5f;
        tailMaxX += w * 0.5f;
        // If width > 3, tail needs to be 1 pixel lower and width restricted by 1.
        if (w > 3)
        {
            tailY -= 1f;
            tailMinX += 1f;
            tailMaxX -= 1f;
        }

        // Flip tail if needed
        float xFlipVal = _flipTailHorizontally ? -1 : 1;
        float yFlipVal = _flipTailVertically ? -1 : 1;
        _bubbleTail.rectTransform.localScale = new Vector2(xFlipVal,yFlipVal);

        // If tail is flipped horizontally, move tail X by 1
        if (_flipTailHorizontally)
        {
            tailMinX += 1f;
            tailMaxX += 1f;
        }
        
        // If tail is flipped vertically, use flip y's sign
        if (_flipTailVertically) tailY *= -1f;

        // Calculate x position
        float tailRawX = Mathf.Lerp(tailMinX, tailMaxX, _bubbleTailPosition);
        float tailOffset = (w % 2 == 0) ? 0.0f : 0.5f;
        float tailX = Mathf.Round(tailRawX - tailOffset) + tailOffset;

        // Place tail in the correct position
        _bubbleTail.rectTransform.localPosition = new Vector2(tailX, tailY);


        // Text Field
        // Text field size is the width or height + 4
        float textFieldWidth = (1 * w) + 4f;
        float textFieldHeight = (1 * h) + 4f;
        _textField.rectTransform.sizeDelta = new Vector2(textFieldWidth, textFieldHeight);

        //To align with pixels, even widths and heights need to be pushed over by half a pixel.
        float textOffsetX = (w % 2 == 0) ? -0.5f : 0f;
        float textOffsetY = (h % 2 == 0) ? -0.5f : 0f;
        //i uh dont think this always works... not sure how to get it to align
        // would probably have to do math with the font itself

        _textField.rectTransform.localPosition = new Vector2(textOffsetX, textOffsetY);
    }

    private void UpdateBubblePosition()
    {
        const int pixelSize = 16;
        float invPixelSize = 1f / pixelSize;

        // Position
        Vector2 newPosition = _parentPosition;

        //increase position based upon size
        newPosition.y += (16 + _bubbleHeight) / (2f * pixelSize); 
        //32 being pixel size * 2
        //16 just being the "0 height bubble size" offset so that its pointing at the position rather than on it

        //apply extra desired offset
        newPosition += _bubbleOffset;

        // Round it to pixel position
        Vector2 alignedPosition = new Vector2(
            Mathf.Round(newPosition.x / invPixelSize) * invPixelSize, 
            Mathf.Round(newPosition.y / invPixelSize) * invPixelSize
        ); 

        // Set position
        transform.position = alignedPosition;
    }

}
