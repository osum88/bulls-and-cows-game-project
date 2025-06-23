window.onload = function() {
    const numbersDiv = document.getElementById("numbers");
    const secretCodeDisplay = document.getElementById("secret-code");
    let hidden_code_array = ["_", "_", "_", "_"];
    secretCodeDisplay.innerText = hidden_code_array.join(" ")

    const numbers = "0123456789⌫".split("");
    numbers.forEach(number => {
        const button = document.createElement("button");
        button.innerText = number;
        if (number == "⌫") {
            button.onclick = () => deleteNumber();
        }
        else {
            button.onclick = () => addNumber(number, button);
        }
        numbersDiv.appendChild(button);
    });

 
    function showModal(message, color) {
        const modal = document.getElementById("gameOverModal");
        const gameMessage = document.getElementById("gameMessage");
        
        gameMessage.innerText = message;
        gameMessage.style.color = color;
        modal.style.display = "flex";  

        const restartButton = document.getElementById("restartButton");
        restartButton.onclick = restartGame;  
    }

    function restartGame() {
        window.location.reload();
    }
    
    function makeGuess(number, button) {
        button.disabled = true;
    
        fetch("/guess", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ number })
        })
        .then(response => response.json())
        .then(updateGameState);
    }

    function lastFilledIndex(array) {
        for (let i = array.length - 1; i >= 0; i--) {
            if (array[i] !== "_") {
                return i;
            }
        }
        return -1; 
    }

    function deleteNumber() {
        let index = lastFilledIndex(hidden_code_array);
        if (index !== -1) {
            const deletedNumber = hidden_code_array[index];
            hidden_code_array[index] = "_";
            secretCodeDisplay.innerText = hidden_code_array.join(" ");

            const buttons = numbersDiv.getElementsByTagName("button");
            for (let btn of buttons) {
                if (btn.innerText === deletedNumber) {
                    btn.disabled = false;
                    break;
                }
            }
        }

    }


    function addNumber(number, button) {
        
        const index = hidden_code_array.indexOf("_");
        if (index !== -1) {
            hidden_code_array[index] = number;
            button.disabled = true;
            secretCodeDisplay.innerText = hidden_code_array.join(" ");
            if (index == 3) {
                //volani makeGuess(hidden_code_array.join(""), button);
            }
        }

    }

    
    function updateGameState(data) {
      
    
        secretCodeDisplay.innerText = hidden_code;

    
        if (status === "won") {
            endGame("VÝHRA\nSlovo bylo: " + word, "green");
        } else if (status === "lost") {
            secretCodeDisplay.innerText = word.split("").join(" "); 
            endGame("PROHRA\nSlovo bylo: " + word, "red");
        }
    }
    
    function endGame(message, color) {
        secretCodeDisplay.style.color = color;
        showModal(message, color);
    }
};
