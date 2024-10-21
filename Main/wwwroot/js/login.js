document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector(".kt-login-v2__form");

    form.addEventListener("submit", function (event) {
        event.preventDefault(); // Impede o envio padrão do formulário

        // Obtém os valores dos campos
        const usernameOrEmail = form.username.value.trim(); // Atualiza para usernameOrEmail
        const password = form.password.value.trim();

        // Verifica se todos os campos estão preenchidos
        if (!usernameOrEmail || !password) {
            swal.fire({
                type: 'error',
                title: 'Oops...',
                text: 'Por favor, preencha todos os campos!',
                timer: 3000,
                showConfirmButton: false,
                onOpen: () => {
                    swal.showLoading();
                }
            });
            return;
        }

        // Se todas as validações passarem, continuar com o login
        const formData = {
            usernameOrEmail, // Atualiza para usernameOrEmail
            password
        };

        // Enviar dados para o controlador usando fetch
        fetch('/Home/Login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        })
            .then(response => {
                if (response.ok) {
                    return response.json(); // Converte a resposta para JSON
                }
                throw new Error('Erro na requisição');
            })
            .then(data => {
                if (data.success) {
                    swal.fire({
                        type: 'success',
                        title: 'Login bem-sucedido',
                        text: 'Você será redirecionado...',
                        timer: 3000,
                        showConfirmButton: false,
                        onOpen: () => {
                            swal.showLoading();
                        }
                    }).then(() => {
                        // Redireciona para a página inicial
                        window.location.href = '/Dashboard'; // Altere conforme necessário
                    });

                } else {
                    swal.fire({
                        type: 'error',
                        title: 'Erro',
                        text: data.message,
                        timer: 3000,
                        showConfirmButton: false,
                        onOpen: () => {
                            swal.showLoading();
                        }
                    });
                }
            })
            .catch(error => {
                console.error('Erro:', error);
                swal.fire({
                    type: 'error',
                    title: 'Erro na comunicação',
                    text: 'Ocorreu um erro ao tentar fazer login.',
                    timer: 3000,
                    showConfirmButton: false,
                    onOpen: () => {
                        swal.showLoading();
                    }
                });
            });
    });
});
