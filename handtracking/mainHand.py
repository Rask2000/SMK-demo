from time import time

import cv2
import socket
import mediapipe as mp
mp_drawing = mp.solutions.drawing_utils
mp_drawing_styles = mp.solutions.drawing_styles
mp_hands = mp.solutions.hands

mp_pose = mp.solutions.pose
hands = mp_hands.Hands(
    model_complexity=1,
    min_detection_confidence=0.3,
    min_tracking_confidence=0.2
    )

cap = cv2.VideoCapture(0, cv2.CAP_DSHOW)

print("Press 'c' to change camera, 's' to use sim video, and 'q' to quit.")
print("Available cameras:")
for i in range(10):
    temp_cap = cv2.VideoCapture(i)
    if temp_cap.isOpened():
        print(f"Camera index {i} is available.")
        temp_cap.release()

cap.set(3, 1280)
cap.set(4, 720)
success, image = cap.read()
h, w, _ = image.shape

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052)

camera_index = 0

while True:
    # Get img frame
    success, image = cap.read()
    image_rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    image_rgb.flags.writeable = False

    results = hands.process(image_rgb)

    # Draw the hand annotations on the image.
    image.flags.writeable = True
    image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
    if results.multi_hand_landmarks:
        for hand_landmarks in results.multi_hand_landmarks:
            mp_drawing.draw_landmarks(
                image,
                hand_landmarks,
                mp_hands.HAND_CONNECTIONS,
                mp_drawing_styles.get_default_hand_landmarks_style(),
                mp_drawing_styles.get_default_hand_connections_style())
        data = [[], []]  # data[0] = hand 1, data[1] = hand 2

        for i, hand_landmarks in enumerate(results.multi_hand_landmarks[:2]):  # max 2 hands
            for lm in hand_landmarks.landmark:
                x = int(lm.x * w)
                y = int(h - (lm.y * h))  
                z = int(lm.z * w)
                data[i].extend([x, y, z])

        hand1_str = ";".join(map(str, data[0]))
        hand2_str = ";".join(map(str, data[1]))
        packet = f"{hand1_str}|{hand2_str}"
        sock.sendto(str.encode(packet), serverAddressPort)
    # Flip the image horizontally for a selfie-view display.
    cv2.imshow('MediaPipe Hands', cv2.flip(image, 1))
    key = cv2.waitKey(1)
    if key == ord('q'):
        break 
    if key == ord('c'):
        # change camera loop through available cameras
        camera_index += 1
        cap.release()
        cap = cv2.VideoCapture(camera_index,  cv2.CAP_DSHOW)
        cap.set(3, 1280)
        cap.set(4, 720)
        success, image = cap.read()
        if not success:
            camera_index = 0
            cap = cv2.VideoCapture(camera_index,  cv2.CAP_DSHOW)
            cap.set(3, 1280)
            cap.set(4, 720)
            success, image = cap.read()
