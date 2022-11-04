import { add, subtract, setbase } from './calculator.js';

function init() {
    const btnAdd = document.querySelector("#btnAdd");
    const btnSubtract = document.querySelector("#btnSubtract");
    const txtA = document.querySelector("input[name='vala']");
    const txtB = document.querySelector("input[name='valb']");
    const result = document.querySelector("#result");
    setbase(document.querySelector("#resturl").value);

    btnAdd.addEventListener("click", async () => {
        result.textContent = await add(+txtA.value, +txtB.value);
    });

    // Feature subtract
    let feat = document.querySelector("#subtract");
    if (feat.value == "true") {
        btnSubtract?.addEventListener("click", async () => {
            result.textContent = await subtract(+txtA.value, +txtB.value);
        });
    }
}

init();
