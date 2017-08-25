using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory {
	public Image[,] invenImage;
	public Image cursor;
	public Image cursorImage;
	public Text cursorText;
	public Image currentImage;
	public Text currentText;

	ItemInfo[,] invenItem;
	float minInvenMove;
	float inputTime;
	int[] currentCursor;
	int[] deltaXY;
	int deltaIndex;
	int prevIndex;

	public Inventory(){
		invenItem = new ItemInfo[Constant.Numbers.maxInvenIndex[1],Constant.Numbers.maxInvenIndex[0]];
		invenImage = new Image[Constant.Numbers.maxInvenIndex[1],Constant.Numbers.maxInvenIndex[0]];
		currentCursor = new int[2]{0, 0};
		deltaXY = new int[2];

		minInvenMove = 0.15f;
		inputTime = minInvenMove;

		// initialize items and images
		for (int i = 0; i < Constant.Numbers.maxInvenIndex [1]; i++) {
			for (int j = 0; j < Constant.Numbers.maxInvenIndex [0]; j++) {
				invenItem [i, j] = null;
				invenImage [i, j] = null;
			}
		}
	}

	public void MoveCursor(float h, float v){
		if (h == 0f && v == 0f) {
			if (inputTime < minInvenMove)
				inputTime = minInvenMove;

		} else {
			if (Mathf.Abs (h) > Mathf.Abs (v)) {
				deltaIndex = 0;
				inputTime += Time.fixedDeltaTime * Mathf.Abs (h);
			} else {
				deltaIndex = 1;
				inputTime += Time.fixedDeltaTime * Mathf.Abs (v);
			}

			if (inputTime > minInvenMove || prevIndex != deltaIndex) {
				if (deltaIndex == 0) {
					if (h > 0) {
						deltaXY [0] = 1;
					} else {
						deltaXY [0] = -1;
					}
				} else {
					if (v > 0) {
						deltaXY [1] = -1;
					} else {
						deltaXY [1] = 1;
					}
				}

				if ((deltaXY [deltaIndex] > 0 && currentCursor [deltaIndex] < Constant.Numbers.maxInvenIndex [deltaIndex] - 1)
					|| (deltaXY [deltaIndex] < 0 && currentCursor [deltaIndex] > 0)) {
					currentCursor [deltaIndex] += deltaXY [deltaIndex];
				}

				cursor.transform.SetParent (invenImage [currentCursor [1], currentCursor [0]].transform.parent);
				Vector3 tmp = cursor.rectTransform.localPosition;
				tmp.x = 25f;
				tmp.y = 25f;
				cursor.rectTransform.localPosition = tmp;

				inputTime = 0;
				prevIndex = deltaIndex;
			}

		}
	}

	public void SetCursor(int x, int y){
		currentCursor [0] = x;
		currentCursor [1] = y;
		cursor.transform.SetParent (invenImage [currentCursor [1], currentCursor [0]].transform.parent);
		Vector3 tmp = cursor.rectTransform.localPosition;
		tmp.x = 25f;
		tmp.y = 25f;
		cursor.rectTransform.localPosition = tmp;
	}

	public void SetCursor(ItemInfo item){
		for (int i = 0; i < Constant.Numbers.maxInvenIndex [1]; i++) {
			for (int j = 0; j < Constant.Numbers.maxInvenIndex [0]; j++) {
				if (item.Equals (invenItem [i, j])) {
					SetCursor (j, i);
					return;
				}
			}
		}
	}

	public void AddItem(ItemInfo item){
		for (int i = 0; i < Constant.Numbers.maxInvenIndex [1]; i++) {
			for (int j = 0; j < Constant.Numbers.maxInvenIndex [0]; j++) {
				if (invenItem [i, j] == null) {
					invenItem [i, j] = item;
					return;
				}
			}
		}
	}

	public void RemoveItem(ItemInfo item){
		for (int i = 0; i < Constant.Numbers.maxInvenIndex [1]; i++) {
			for (int j = 0; j < Constant.Numbers.maxInvenIndex [0]; j++) {
				if (item.Equals (invenItem [i, j])) {
					invenItem [i, j] = null;
					return;
				}
			}
		}
	}
}
