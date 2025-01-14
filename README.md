# 🎮 GoodJobGames Internship Case

📌 **Currently Live:**  
I developed a similar mechanic project that is live in the Karaca mobile app. You can explore it here:  
👉 [Mobile Karaca App - Bubble Blast](https://omerozerf.notion.site/Mobile-Karaca-App-Bubble-Blast-162443d669db81fcbf7cd233dd9de7b7)

---
---
---

Welcome to the **Color Block-Based Game System**! This project is a visually engaging, performance-optimized, and dynamic game system built with Unity. 🧱✨

---

## 🌈 **Project Summary**

The game provides a playing field of colored blocks, where players interact to eliminate groups of the same color while earning points. 🚀

---

## 🎯 **Project Purpose**

- Create an interactive game with a scoring mechanism.
- Dynamically manage and shuffle the game board for continuous playability.

---

## 🛠️ **Technical Details**

### **Technologies Used**
- **Unity Game Engine: Version 6000.0.32f1**
- **C#**
- **Third-Party Libraries**:
  - DOTween (for animations)
  - Cysharp UniTask (for asynchronous operations)

### **Key Features**
1. **Blocks and Cells** 🧱  
   - Each block has a unique color and is placed in cells.  
   - Positions dynamically update with visual effects.
   
2. **Flood Fill Mechanism** 🌊  
   - Groups blocks of the same color for elimination.  

3. **Pooling Management** 🔄  
   - Optimizes memory usage with an `ObjectPool<T>` structure.  

4. **Animations** 🎥  
   - Smooth animations for movement, scaling, and effects.  

5. **Board Layout** 🎲  
   - Dynamically arranged cells and blocks with automatic refills for empty spaces.  

6. **Shuffle Mechanism (With Clusters)** 🔀  
   - Groups blocks by color and shuffles them when no moves are possible.

---

## ⚙️ **Performance Optimizations**

- **Unity Profiler**: Optimized CPU, GPU, and memory usage.  
- **Object Pooling**: Efficient memory management by recycling objects.  
- **DOTween**: Smooth and reusable animations.  
- **Asynchronous Operations**: Improved user experience by minimizing delays.

---

## 📋 **Functional Descriptions**

### **Block Management** 🏗️
- **Classes**:  
  - `Block`: Manages block properties and cell interactions.  
  - `BlockVisual`: Adjusts visual elements.  
  - `BlockAnimation`: Controls movement and scaling animations.  

### **Cells** 🧩
- Act as containers for blocks.  
- Utilize `FloodFillHelper` to group blocks of the same color.

### **Game Board (Board)** 🕹️
- Dynamically creates and arranges cells.  
- Processes all interactions asynchronously.  
- Refills empty cells with new blocks.  
- Includes animations for clearing areas.

### **Shuffle Mechanism** 🔄
1. **Move Control**:
   - Detects possible moves; initiates shuffling if none exist.  
2. **Cluster Grouping**:
   - Groups blocks by color using `Dictionary<BlockColor, List<Block>>`.  
3. **Random Shuffling**:
   - Redistributes grouped blocks randomly.  
4. **Result**:
   - Creates a refreshed board layout for continued gameplay.  

---

## 🎉 **Project Contributions**

- **Engaging User Experience** 🌟  
  - Colorful visuals, shuffle mechanisms, and animations.  

- **Performance Optimization** 🚀  
  - Efficient resource utilization with Object Pooling and asynchronous programming.

- **Dynamic Gameplay** 💫  
  - Prevents stagnation with a seamless shuffle process.

---

Feel free to explore the project and contribute! 🎮✨
