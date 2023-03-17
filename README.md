# rm_vision_simulator
基于 Unity 实现的 [rm_vision](https://github.com/chenjunnn/rm_vision) 项目视觉仿真环境

## 部署指南
1. clone 该项目至本地硬盘

    ```
    git clone https://github.com/chenjunnn/rm_vision_simulator
    ```

2. 下载 [Unity 2021.3.11f1c1](https://unity.com/cn/download)
3. 下载 [ros2-for-unity 1.2.0 release](https://github.com/RobotecAI/ros2-for-unity/releases/download/1.2.0/Ros2ForUnity_humble_windows10.zip) 并将其解压至 `rm_vision_simulator/Assets/`
4. 根据 [ROS文档](https://docs.ros.org/en/humble/Installation/Windows-Install-Binary.html) 安装 ROS2 Humble
5. 引入ROS环境变量后打开 Unity Hub，在项目中打开 rm_vision_simulator

    ```
    C:\dev\ros2_humble\setup.ps1
    & 'C:\Program Files\Unity Hub\Unity Hub.exe'
    ```
