using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Character = CharacterDictionarySO.Character;

namespace cherrydev
{
    //[CreateAssetMenu(menuName = "Scriptable Objects/Nodes/Answer Node", fileName = "New Answer Node")]
    public class AnswerNode : Node
    {
        private const int amountOfAnswers = 3;

        public List<Answer> answers = new List<Answer>();

        [HideInInspector] public SentenceNode parentSentenceNode;
        [HideInInspector] public SentenceNode[] childSentenceNodes;

        private const float labelFieldSpaceChoice = 15f;
        private const float textFieldWidthChoice = 130f;

        private const float labelFieldSpace = 40f;
        private const float textFieldWidth = 120f;

        private const float answerNodeWidth = 200f;
        private const float answerNodeHeight = 145f;


#if UNITY_EDITOR

        /// <summary>
        /// Answer node initialisation method
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="nodeName"></param>
        /// <param name="nodeGraph"></param>
        public override void Initialise(Rect rect, string nodeName, DialogNodeGraph nodeGraph)
        {
            base.Initialise(rect, nodeName, nodeGraph);

            childSentenceNodes = new SentenceNode[amountOfAnswers];

            for (int i = 0; i < amountOfAnswers; i++)
            {
                Answer newAnswer = new Answer();
                newAnswer.answer = "";
                answers.Add(newAnswer);
            }
        }

        /// <summary>
        /// Draw Answer Node method
        /// </summary>
        /// <param name = "nodeStyle" ></ param >
        /// < param name="lableStyle"></param>
        public override void Draw(GUIStyle nodeStyle, GUIStyle lableStyle)
        {
            base.Draw(nodeStyle, lableStyle);

            nodeTitle = $"{count} {character.ToString()}";

            if (nodeTitle != prevTitle) {
                prevTitle = nodeTitle;
                name = nodeTitle;
                AssetDatabase.SaveAssets();
            }

            rect.size = new Vector2(answerNodeWidth, answerNodeHeight);

            GUILayout.BeginArea(rect, nodeStyle);
            EditorGUILayout.LabelField(nodeTitle, lableStyle);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", GUILayout.Width(labelFieldSpace));
            character = (CharacterDictionarySO.CharacterID) EditorGUILayout.EnumPopup(character, GUILayout.Width(textFieldWidth));
            EditorGUILayout.EndHorizontal();

            DrawAnswerLine(1, EditorIcons.GreenDot);
            DrawAnswerLine(2, EditorIcons.GreenDot);
            DrawAnswerLine(3, EditorIcons.GreenDot);
            //DrawAnswerLine(4, EditorIcons.GreenDot);

            GUILayout.EndArea();
        }

        private void DrawAnswerLine(int answerNumber, string iconPathOrName)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{answerNumber}. ", GUILayout.Width(labelFieldSpaceChoice));
            answers[answerNumber - 1].answer = EditorGUILayout.TextField(answers[answerNumber - 1].answer, GUILayout.Width(textFieldWidthChoice));
            EditorGUILayout.LabelField(EditorGUIUtility.IconContent(iconPathOrName), GUILayout.Width(labelFieldSpaceChoice));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Adding nodeToAdd Node to the parentSentenceNode field
        /// </summary>
        /// <param name="nodeToAdd"></param>
        /// <returns></returns>
        public override bool AddToParentConnectedNode(Node nodeToAdd)
        {
            if (nodeToAdd.GetType() == typeof(SentenceNode))
            {
                parentSentenceNode = (SentenceNode)nodeToAdd;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Adding nodeToAdd Node to the childSentenceNodes array
        /// </summary>
        /// <param name="nodeToAdd"></param>
        /// <returns></returns>
        public override bool AddToChildConnectedNode(Node nodeToAdd)
        {
            SentenceNode sentenceNodeToAdd;

            if (nodeToAdd.GetType() != typeof(AnswerNode))
            {
                sentenceNodeToAdd = (SentenceNode)nodeToAdd;
            }
            else
            {
                return false;
            }

            char letter = 'a';
            for (int i = 0; i < amountOfAnswers; i++)
            {
                if (childSentenceNodes[i] == null && sentenceNodeToAdd.parentNode == null)
                {
                    childSentenceNodes[i] = sentenceNodeToAdd;
                    sentenceNodeToAdd.count = count + 1;
                    sentenceNodeToAdd.SetLetter($"{(char)(letter + i)}");
                    return true;
                }
            }

            return false;
        }

        public bool AreChildrenMaxedOut() {
            return childSentenceNodes[amountOfAnswers - 1] != null;
        }

#endif
    }
}