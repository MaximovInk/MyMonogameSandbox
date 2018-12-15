﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MySandbox.Core
{
    [Serializable]
    public class GameObject
    {
        public Texture2DSheet Sprite{ get; set; }
        
        protected Rectangle current_element;

        public Vector2 Position { get; set; }
        
        public Color color = Color.White;

        public float Rotation { get; set; }
        [JsonIgnore]
        public bool FlipX { get { return flip_x; } set{ flip_x = value; SetEffects(); } }
        [JsonIgnore]
        protected bool flip_x = false;
        [JsonIgnore]
        public bool FlipY { get { return flip_y; } set{ flip_y = value; SetEffects(); } }
        [JsonIgnore]
        protected bool flip_y = false;
        
        public float Scale = 1;
        [JsonIgnore]
        public Vector2 Bounds { get { return new Vector2(current_element.Width * Scale, current_element.Height * Scale); } }

        public bool DrawThis = true;

        protected SpriteEffects effects;

        protected void SetEffects()
        {
            effects = (FlipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None) | (FlipY ? SpriteEffects.FlipVertically : SpriteEffects.None);
        }

        protected List<Component> components = new List<Component>();

        public virtual List<GameObject> Childrens { get; protected set; } = new List<GameObject>();

        public bool update = false;

        [JsonConstructor]
        protected GameObject()
        {
            Console.WriteLine("gg");
        }

        public void UpdateTexture()
        {
            if (Sprite == null || Sprite.TexturePath == null)
                return;
                Sprite.sheet = Core.content.Load<Texture2D>(Sprite.TexturePath);
            SetElement(0);
        }

        public GameObject(Texture2DSheet atlas, Vector2 position, float rotation = 0 , bool update = false)
        {
            Sprite = atlas;
            Position = position;
            SetElement(0);
            this.update = update;
        }

        public Component AddComponent(Component component)
        {
            components.Add(component);
            component.Construct(this);
            return component;
        }

        public void RemoveComponent(Component component)
        {
            components.Remove(component);
        }

        public void RemoveComponent(int index)
        {
            components.RemoveAt(index);
        }

        public void SetElement(int index)
        {
            if (Sprite == null || Sprite.elements.Length <= index)
                return;

            current_element = Sprite.elements[index];
        }

        public virtual void Draw()
        {
            if (!DrawThis)
                return;
            if (Sprite != null && Sprite.sheet != null)
                Core.spriteBatch.Draw(Sprite.sheet, Position, current_element, color, Rotation, Vector2.Zero, Scale, effects, 0);

            for (int i = 0; i < Childrens.Count; i++)
            {
                Childrens[i].Draw();
            }
        }

        public void Update(GameTime gameTime)
        {
            if(update)
            for (int i = 0; i < components.Count; i++)
            {
                    components[i].Update(gameTime);
            }
            for (int i = 0; i < Childrens.Count; i++)
            {
                Childrens[i].Update(gameTime);
            }
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {

        }

        public override string ToString()
        {
            return "GameObject [Pos:" + Position.X +  "," + Position.Y +"]";
        }

        public Vector2 GetCentredPosition()
        {
            return new Vector2(Position.X + Bounds.X/2, Position.Y+Bounds.Y/2);
        }

        public T GetComponent<T>() where T : Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components is T)
                {
                    return components[i] as T;
                }
            }

            return null;
        }
    }
}
