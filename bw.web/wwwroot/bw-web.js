window.bw_web = {
    ignore: {}
    , pbkdf2: async function (b64pass, b64salt, iterations) {
        const passBuf = bw_web.fromB64ToArray(b64pass);
        const saltBuf = bw_web.fromB64ToArray(b64salt);

        const impKey = await crypto.subtle.importKey(
            'raw',              // format
            passBuf,        // keyData
            { name: 'PBKDF2' }, // algorithm
            false,              // extractable
            ['deriveBits']      // usages
        );

        const pbkdf2Params = {
            name: 'PBKDF2',
            hash: { name: "SHA-256" },
            salt: saltBuf,
            iterations: iterations
        };

        const bits = await crypto.subtle.deriveBits(pbkdf2Params, impKey, 256);
        const b64 = bw_web.fromBufferToB64(bits);
        console.log("Generated bits: " + bits.byteLength + " ; " + b64);
        return b64;
    }
    , fromB64ToArray(str) {
        const binaryString = window.atob(str);
        const bytes = new Uint8Array(binaryString.length);
        for (let i = 0; i < binaryString.length; i++) {
            bytes[i] = binaryString.charCodeAt(i);
        }
        return bytes;
    }
    , fromBufferToB64(buffer) {
        let binary = '';
        const bytes = new Uint8Array(buffer);
        for (let i = 0; i < bytes.byteLength; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }
};
