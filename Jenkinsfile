pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                git branch: 'main', url: 'https://github.com/KAIF254/devops-git.git'
            }
        }

        stage('Build Docker Image') {
            steps {
                bat 'docker build -t kaif4u/onlinebookstore:latest .'
            }
        }
    }
}
