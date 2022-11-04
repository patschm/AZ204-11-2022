let basepath = "https://localhost:8001/calculator/";

export function setbase(baseurl) {
    basepath = baseurl;
}
export async function add(a, b) {
    return await call(a, b, "add");
}
export async function subtract(a, b) {
    return await call(a, b, "subtract");
}

async function call(a, b, subpath) {
    let response = await fetch(basepath + subpath + `?a=${a}&b=${b}`);
    if (response.ok) {
        let result = await response.json();
        return result;
    }
}