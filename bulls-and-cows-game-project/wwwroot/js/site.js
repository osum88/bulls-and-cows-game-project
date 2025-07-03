window.onload = function() {
    const numbersDiv = document.getElementById("numbers");
    const secretCodeDisplay = document.getElementById("secret-code");
    let hidden_code_array = ["_", "_", "_", "_"];
    secretCodeDisplay.innerText = hidden_code_array.join(" ");
    const guessesTableBody = $('#tableBody');
    const guessesHistory = [];

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

    for (let i = 0; i < 10; i++) {
        const newRow = `
            <tr>
                <td> </td>
                <td> </td>
                <td> </td>
            </tr>
        `;
        guessesTableBody.append(newRow); 
    }

    function makeGuess(guess) {
        fetch("/api/guess", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(guess) 
        })
        .then(response => response.json())
        .then(data => updateGameState(data, guess));
    }
    $('#endGameButton').on('click', function () {
        if (confirm('Do you want to end the game?')) {
            fetch('/api/Guess/EndGame', { 
                method: 'POST',
                headers: { "Content-Type": "application/json" }
            })
            .then(response => response.json())
            .then(data => updateGameState(data, ""));
        }
    });

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
                makeGuess(hidden_code_array.join(""));
            }
        }
    }

    function resetInput() {
        hidden_code_array = ["_", "_", "_", "_"];
        secretCodeDisplay.innerText = hidden_code_array.join(" ");

        const buttons = numbersDiv.getElementsByTagName("button");
        for (let btn of buttons) {
            if (btn.innerText !== "⌫") {
                btn.disabled = false;
            }
        }
    }
    
    function updateGameState(data, guess) {
        if (guess != "") {
            addGuessToTable(guess, data.bulls, data.cows, data);
        }
        resetInput();
        document.getElementById("actualAttempts").innerText = data.attempts;
        if (data.isEndGame) {
            let message = data.resultGame ? "You won!" : "You lost!";
            document.getElementById("finalAttempts").innerText = data.attempts;
            document.getElementById("revealedCode").innerText = data.secretCode;
            document.getElementById("gameMessage").innerText = message;
            document.getElementById("time").innerText = data.resultTime;
            document.getElementById("gameOverModal").style.display = "flex";

        }
    }

    function addGuessToTable(guessCode, bulls, cows, data) {
        const tableBody = document.getElementById('tableBody');

        const newRow = document.createElement('tr');
        newRow.innerHTML = `
            <td>${guessCode}</td>
            <td>${bulls}</td>
            <td>${cows}</td>
        `;

        if (data.attempts <= 10) {
            tableBody.insertBefore(newRow, tableBody.rows[data.attempts - 1]);
            tableBody.deleteRow(tableBody.rows.length - 1);
        }
        else
        {
            tableBody.appendChild(newRow);
            tableBody.deleteRow(0);
        }
    }
};
