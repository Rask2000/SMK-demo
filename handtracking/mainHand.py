from cvzone.HandTrackingModule import HandDetector
import cv2
import socket

cap = cv2.VideoCapture(0)
cap.set(3, 1280)
cap.set(4, 720)
success, img = cap.read()
h, w, _ = img.shape
detector = HandDetector(detectionCon=0.8, maxHands=2)

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052)

camera_index = 0

while True:
    # Get image frame
    success, img = cap.read()
    # Find the hand and its landmarks
    hands, img = detector.findHands(img)  # with draw
    # hands = detector.findHands(img, draw=False)  # without draw
    data = []

    if hands:
        # Hand 1
        hand = hands[0]
        lmList = hand["lmList"]  # List of 21 Landmark points
        for lm in lmList:
            data.extend([lm[0], h - lm[1], lm[2]])

        sock.sendto(str.encode(str(data)), serverAddressPort)

    # Display
    cv2.imshow("Image", img)
    
    key = cv2.waitKey(1)
    if key == ord('q'):
        break
    if key == ord('c'):

        # change camera loop through available cameras
        camera_index += 1
        cap.release()
        cap = cv2.VideoCapture(camera_index)
        cap.set(3, 1280)
        cap.set(4, 720)
        success, img = cap.read()
        if not success:
            camera_index = 0
            cap = cv2.VideoCapture(camera_index)
            cap.set(3, 1280)
            cap.set(4, 720)
            success, img = cap.read()


    if key == ord('s'):
        # use standing video and loop it
        cap.release()
        cap = cv2.VideoCapture("standing.mp4")
        cap.set(3, 1280)
        cap.set(4, 720)
        success, img = cap.read()
        while True:
            success, img = cap.read()
            if not success:
                cap.set(cv2.CAP_PROP_POS_FRAMES, 0)  # Loop the video
                continue
            hands, img = detector.findHands(img)
            data = []
            if hands:
                hand = hands[0]
                lmList = hand["lmList"]
                for lm in lmList:
                    data.extend([lm[0], h - lm[1], lm[2]])
                sock.sendto(str.encode(str(data)), serverAddressPort)
            cv2.imshow("Image", img)
            if cv2.waitKey(1) & 0xFF == ord('q'):
                break
